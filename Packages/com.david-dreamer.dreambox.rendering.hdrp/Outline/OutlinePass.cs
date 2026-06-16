// Copyright (c) Saber BGS 2022. All rights reserved.
// ---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Dreambox.Rendering.Core;
using System.Runtime.InteropServices;

namespace Dreambox.Rendering.HDRP
{
	public class OutlinePass : CustomPass
	{
		[field: SerializeField]
		private Shader Shader { get; set; }

		[field: SerializeField]
		private GraphicsFormat MaskGraphicsFormat { get; set; } = GraphicsFormat.R16_UInt;

		[field: SerializeField]
		private GraphicsFormat MainGraphicsFormat { get; set; } = GraphicsFormat.R16G16B16A16_SFloat;

		[field: SerializeField]
		public OutlineVariant[] Variants { get; private set; }

		[field: SerializeField]
		private float Width { get; set; }

		private Material Material { get; set; }

		private ComputeBuffer VariantsBuffer { get; set; }

		public RTHandle MaskRT { get; set; }

		public RTHandle JumpBuffer1RT { get; set; }

		public RTHandle JumpBuffer2RT { get; set; }

		public HashSet<OutlineRenderer> Targets { get; } = new();

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			Material = CoreUtils.CreateEngineMaterial(Shader);

			VariantsBuffer = new ComputeBuffer(Variants.Length, Marshal.SizeOf<OutlineVariant>());
			VariantsBuffer.SetData(Variants);

			Material.SetBuffer(OutlineShaderVariable.VariantsBuffer, VariantsBuffer);

			TextureDimension dimension = TextureDimension.Tex2D;
			int slices = TextureXR.slices;

			MaskRT = RTHandles.Alloc(
				Vector3.one,
				dimension: dimension,
				slices: slices,
				colorFormat: MaskGraphicsFormat,
				autoGenerateMips: false,
				useDynamicScale: true,
				name: "Outline.Mask");

			JumpBuffer1RT = RTHandles.Alloc(
				Vector3.one,
				dimension: dimension,
				slices: slices,
				colorFormat: MainGraphicsFormat,
				autoGenerateMips: false,
				useDynamicScale: true,
				name: "Outline.JumpBuffer1");

			JumpBuffer2RT = RTHandles.Alloc(
				Vector3.one,
				dimension: dimension,
				slices: slices,
				colorFormat: MainGraphicsFormat,
				autoGenerateMips: false,
				useDynamicScale: true,
				name: "Outline.JumpBuffer2");
		}

		protected override void Cleanup()
		{
			CoreUtils.Destroy(Material);
			VariantsBuffer.Dispose();
			MaskRT.Release();
			JumpBuffer1RT.Release();
			JumpBuffer2RT.Release();
		}

		protected override void Execute(CustomPassContext context)
		{
			CommandBuffer commandBuffer = context.cmd;

			float maxPixelWidth = Screen.height * Width;
			int iterations = Mathf.CeilToInt(Mathf.Log(maxPixelWidth, 2f)) - 1;

			VariantsBuffer.SetData(Variants);

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
			}

			void Initialize()
			{
				RTHandle startBuffer = iterations % 2 == 0 ? JumpBuffer2RT : JumpBuffer1RT;
				Blitter.BlitTexture(commandBuffer, MaskRT, startBuffer, Material, OutlineShaderPass.Init);
			}

			void JumpFlood()
			{
				for (int i = iterations; i >= 0; i--)
				{
					RTHandle source, target;
					if (i % 2 == 1)
					{
						source = JumpBuffer1RT;
						target = JumpBuffer2RT;
					}
					else
					{
						source = JumpBuffer2RT;
						target = JumpBuffer1RT;
					}

					float stepWidth = Mathf.Pow(2, i);
					commandBuffer.SetGlobalFloat(OutlineShaderVariable.StepWidth, stepWidth);

					Blitter.BlitTexture(commandBuffer, source, target, Material, OutlineShaderPass.JumpFlood);
				}
			}

			void Decode()
			{
				Blitter.BlitTexture(commandBuffer, JumpBuffer1RT, context.cameraColorBuffer, Material, OutlineShaderPass.Decode);
			}
		}
	}
}
