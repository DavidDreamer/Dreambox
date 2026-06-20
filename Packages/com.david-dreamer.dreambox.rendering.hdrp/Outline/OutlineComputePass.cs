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
	public class OutlineComputePass : CustomPass, IOutlinePass
	{
		private const int NUM_THREADS = 8;

		[field: SerializeField]
		private ComputeShader ComputeShader { get; set; }

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

		public RTHandle JumpFloodRT { get; set; }

		public HashSet<OutlineRenderer> Targets { get; } = new();

		private int Iterations { get; set; }

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			Material = CoreUtils.CreateEngineMaterial(Shader);

			VariantsBuffer = new ComputeBuffer(Variants.Length, Marshal.SizeOf<OutlineVariant>());
			VariantsBuffer.SetData(Variants);

			Material.SetBuffer(OutlineShaderVariable.VariantsBuffer, VariantsBuffer);

			CalculateIterationsCount();

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

			JumpFloodRT = RTHandles.Alloc(
				Vector3.one,
				dimension: dimension,
				slices: slices,
				colorFormat: MainGraphicsFormat,
				autoGenerateMips: false,
				useDynamicScale: true,
				enableRandomWrite: true,
				name: "Outline.JumpFlood");
		}

		protected override void Cleanup()
		{
			CoreUtils.Destroy(Material);
			VariantsBuffer.Dispose();
			MaskRT.Release();
			JumpFloodRT.Release();
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
				commandBuffer.SetComputeTextureParam(ComputeShader, 0, "MaskTexture", MaskRT);
				commandBuffer.SetComputeTextureParam(ComputeShader, 0, "JumpFloodTexture", JumpFloodRT);
				commandBuffer.DispatchCompute(ComputeShader, OutlineKernel.Initialize, Screen.width / NUM_THREADS, Screen.height / NUM_THREADS, 1);
			}

			void JumpFlood()
			{
				for (int i = Iterations; i >= 0; i--)
				{
					int stepWidth = (int)Mathf.Pow(2, i);
					commandBuffer.SetComputeIntParam(ComputeShader, OutlineShaderVariable.StepWidth, stepWidth);
					commandBuffer.SetComputeVectorParam(ComputeShader, "Resolution", new Vector2(Screen.width, Screen.height));
					commandBuffer.SetComputeTextureParam(ComputeShader, 1, "JumpFloodTexture", JumpFloodRT);
					commandBuffer.DispatchCompute(ComputeShader, OutlineKernel.JumpFlood, Screen.width / NUM_THREADS, Screen.height / NUM_THREADS, 1);
				}
			}

			void Decode()
			{
				Blitter.BlitTexture(commandBuffer, JumpFloodRT, context.cameraColorBuffer, Material, OutlineShaderPass.Decode);
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
