using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Dreambox.Rendering.Core;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;

namespace Dreambox.Rendering.HDRP
{
	public class OutlinePass : CustomPass, IOutlinePass
	{
		[field: SerializeField]
		private Shader Shader { get; set; }

		[field: SerializeField]
		private GraphicsFormat MaskGraphicsFormat { get; set; } = GraphicsFormat.R16_UInt;

		[field: SerializeField]
		private GraphicsFormat MainGraphicsFormat { get; set; } = GraphicsFormat.R16G16B16A16_SFloat;

		[field: SerializeField]
		public OutlineVariant[] Variants { get; private set; }

		private Material Material { get; set; }

		private ComputeBuffer VariantsBuffer { get; set; }

		public RTHandle MaskRT { get; set; }

		public RTHandle JumpFlood1RT { get; set; }

		public RTHandle JumpFlood2RT { get; set; }

		public HashSet<OutlineRenderer> Targets { get; } = new();

		private int Iterations { get; set; }

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			Material = CoreUtils.CreateEngineMaterial(Shader);

			VariantsBuffer = new ComputeBuffer(Variants.Length, Marshal.SizeOf<OutlineVariant>());
			VariantsBuffer.SetData(Variants);

			Material.SetBuffer(OutlineShaderVariable.VariantsBuffer, VariantsBuffer);

			CalculateIterationsCount();

			MaskRT = Alloc(MaskGraphicsFormat, false, "Outline.Mask");
			JumpFlood1RT = Alloc(MainGraphicsFormat, true, "Outline.JumpFlood1");
			JumpFlood2RT = Alloc(MainGraphicsFormat, true, "Outline.JumpFlood2");

			static RTHandle Alloc(GraphicsFormat graphicsFormat, bool enableRandomWrite, string name) => RTHandles.Alloc(
				Vector3.one,
				dimension: TextureDimension.Tex2D,
				slices: TextureXR.slices,
				colorFormat: graphicsFormat,
				useDynamicScale: true,
				enableRandomWrite: enableRandomWrite,
				name: name);
		}

		protected override void Cleanup()
		{
			CoreUtils.Destroy(Material);
			VariantsBuffer.Dispose();
			MaskRT.Release();
			JumpFlood1RT.Release();
			JumpFlood2RT.Release();
		}

		private void CalculateIterationsCount()
		{
			int maxWidth = 0;
			foreach (OutlineVariant variant in Variants)
			{
				maxWidth = Math.Max(maxWidth, variant.Width);
			}

			Iterations = Mathf.CeilToInt(Mathf.Log(maxWidth, 2f)) - 1;
		}

		protected override void Execute(CustomPassContext context)
		{
			CommandBuffer commandBuffer = context.cmd;

			RefreshVariantsData();

			Mask();
			Initialize();
			JumpFlood();
			Decode();

			void Mask()
			{
				commandBuffer.SetRenderTarget(MaskRT);
				commandBuffer.ClearRenderTarget(true, true, Color.clear);

				foreach (OutlineRenderer target in Targets)
				{
					commandBuffer.SetGlobalInteger(OutlineShaderVariable.Variant, target.Variant + 1);
					Texture baseMap = target.Material.HasTexture(OutlineShaderVariable.BaseMap) ?
						target.Material.GetTexture(OutlineShaderVariable.BaseMap) : Texture2D.whiteTexture;
					commandBuffer.SetGlobalTexture(OutlineShaderVariable.BaseMap, baseMap);
					commandBuffer.DrawMesh(target.Mesh, target.Matrix, Material, 0, OutlineShaderPass.Mask);
				}

				commandBuffer.SetGlobalTexture(OutlineShaderVariable.OutlineMaskTexture, MaskRT);
			}

			void Initialize()
			{
				RTHandle startBuffer = Iterations % 2 == 0 ? JumpFlood2RT : JumpFlood1RT;
				Blitter.BlitTexture(commandBuffer, MaskRT, startBuffer, Material, OutlineShaderPass.Initialize);
			}

			void JumpFlood()
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

					float stepWidth = Mathf.Pow(2, i);
					commandBuffer.SetGlobalFloat(OutlineShaderVariable.StepWidth, stepWidth);

					Blitter.BlitTexture(commandBuffer, source, target, Material, OutlineShaderPass.JumpFlood);
				}
			}

			void Decode()
			{
				Blitter.BlitTexture(commandBuffer, JumpFlood1RT, context.cameraColorBuffer, Material, OutlineShaderPass.Decode);
			}
		}

		[Conditional("UNITY_EDITOR")]
		private void RefreshVariantsData()
		{
			CalculateIterationsCount();
			VariantsBuffer.SetData(Variants);
		}
	}
}
