using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using Dreambox.Rendering.Core;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;

namespace Dreambox.Rendering.HDRP
{
	internal abstract class OutlinePipeline : IOutlinePipeline
	{
		private const GraphicsFormat MaskGraphicsFormat = GraphicsFormat.R8_UInt;

		private const GraphicsFormat JumpFloodGraphicsFormat = GraphicsFormat.R16G16_SInt;

		protected Material MaskMaterial { get; }

		protected Material Material { get; }

		protected ComputeBuffer VariantsBuffer { get; }

		protected RTHandle MaskRT { get; }

		protected RTHandle JumpFlood1RT { get; }

		protected RTHandle JumpFlood2RT { get; }

		protected int Iterations { get; set; }

		public OutlinePipeline(Shader shader, OutlineVariant[] variants)
		{
			Shader maskShader = Shader.Find("Hidden/Dreambox/Outline/Mask");
			MaskMaterial = CoreUtils.CreateEngineMaterial(maskShader);

			Material = CoreUtils.CreateEngineMaterial(shader);

			VariantsBuffer = new ComputeBuffer(variants.Length, Marshal.SizeOf<OutlineVariant>());
			VariantsBuffer.SetData(variants);

			Material.SetBuffer(OutlineShaderVariable.VariantsBuffer, VariantsBuffer);

			MaskRT = Alloc(MaskGraphicsFormat, false, "Outline.Mask");
			JumpFlood1RT = Alloc(JumpFloodGraphicsFormat, true, "Outline.JumpFlood1");
			JumpFlood2RT = Alloc(JumpFloodGraphicsFormat, true, "Outline.JumpFlood2");

			CalculateIterationsCount(variants);

			static RTHandle Alloc(GraphicsFormat graphicsFormat, bool enableRandomWrite, string name) => RTHandles.Alloc(
				Vector3.one,
				dimension: TextureDimension.Tex2D,
				slices: TextureXR.slices,
				colorFormat: graphicsFormat,
				useDynamicScale: true,
				enableRandomWrite: enableRandomWrite,
				name: name);
		}

		public void Dispose()
		{
			CoreUtils.Destroy(Material);

			VariantsBuffer.Dispose();

			MaskRT.Release();
			JumpFlood1RT.Release();
			JumpFlood2RT.Release();
		}

		public void CalculateIterationsCount(OutlineVariant[] variants)
		{
			int maxWidth = 0;
			foreach (OutlineVariant variant in variants)
			{
				maxWidth = Math.Max(maxWidth, variant.Width);
			}

			Iterations = Mathf.CeilToInt(Mathf.Log(maxWidth, 2f)) - 1;
		}

		public void Mask(CommandBuffer commandBuffer, HashSet<OutlineRenderer> renderers)
		{
			commandBuffer.SetRenderTarget(MaskRT);
			commandBuffer.ClearRenderTarget(true, true, Color.clear);

			foreach (OutlineRenderer renderer in renderers)
			{
				commandBuffer.SetGlobalInteger(OutlineShaderVariable.Variant, renderer.Variant + 1);
				Texture baseMap = renderer.Material.HasTexture(OutlineShaderVariable.BaseMap) ?
					renderer.Material.GetTexture(OutlineShaderVariable.BaseMap) : Texture2D.whiteTexture;
				commandBuffer.SetGlobalTexture(OutlineShaderVariable.BaseMap, baseMap);
				commandBuffer.DrawMesh(renderer.Mesh, renderer.Matrix, MaskMaterial, 0, OutlineShaderPass.Mask);
			}
		}

		public abstract void Initialize(CommandBuffer commandBuffer);

		public void JumpFlood(CommandBuffer commandBuffer)
		{
			for (int i = Iterations; i >= 0; i--)
			{
				RTHandle source, target;
				if (i % 2 == 1)
				{
					source = JumpFlood1RT;
					target = JumpFlood2RT;
				}
				else
				{
					source = JumpFlood2RT;
					target = JumpFlood1RT;
				}

				int stepWidth = (int)Mathf.Pow(2, i);

				JumpFlood(commandBuffer, source, target, stepWidth);
			}
		}

		public abstract void JumpFlood(CommandBuffer commandBuffer, RTHandle source, RTHandle target, int stepWidth);

		public void Decode(CommandBuffer commandBuffer, RTHandle target)
		{
			commandBuffer.SetGlobalTexture(OutlineShaderVariable.MaskTexture, MaskRT);
			Blitter.BlitTexture(commandBuffer, JumpFlood1RT, target, Material, OutlineShaderPass.Decode);
		}

		[Conditional("UNITY_EDITOR")]
		public void RefreshVariants(OutlineVariant[] variants)
		{
			CalculateIterationsCount(variants);
			VariantsBuffer.SetData(variants);
		}
	}
}
