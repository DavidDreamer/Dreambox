using System.Diagnostics;
using Dreambox.Rendering.Core;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Dreambox.Rendering.HDRP
{
	public class BlurPass : CustomPass
	{
		public enum OutputTarget
		{
			Camera,
			Texture
		}

		[field: SerializeField]
		public BlurMode Mode { get; private set; }

		[field: SerializeField]
		public BlurSettings Settings { get; private set; }

		[field: SerializeField]
		public OutputTarget Target { get; private set; }

		private IBlur Blur { get; set; }

		protected override bool executeInSceneView => false;

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			Setup();
		}

		private void Setup()
		{
			name = $"Blur - {Mode}";

			GraphicsFormat graphicsFormat = HDRenderPipelineAssetUtils.GetColorBufferGraphicsFormat();

			Blur = Initialize();

			IBlur Initialize()
			{
				switch (Mode)
				{
					case BlurMode.Box:
						return new BoxBlur(Settings, graphicsFormat);
					case BlurMode.Gaussian:
						return new GaussianBlur(Settings, graphicsFormat);
					case BlurMode.Kawase:
						return new KawaseBlur(Settings, graphicsFormat);
					default:
						return new GaussianBlur(Settings, graphicsFormat);
				}
			}
		}

		protected override void Cleanup()
		{
			Blur.Dispose();
		}

		[Conditional("UNITY_EDITOR")]
		public void Reset()
		{
			Cleanup();
			Setup();
		}

		protected override void Execute(CustomPassContext context)
		{
			base.Execute(context);

			CommandBuffer commandBuffer = context.cmd;

			Blur.Execute(commandBuffer, context.cameraColorBuffer);

			switch (Target)
			{
				case OutputTarget.Camera:
					commandBuffer.SetRenderTarget(context.cameraColorBuffer);
					Blitter.BlitTexture(commandBuffer, Blur.Result, new Vector4(1, 1, 0, 0), 0, false);
					break;
				case OutputTarget.Texture:
					commandBuffer.SetGlobalTexture(BlurShaderVariable.BlurTexture, Blur.Result);
					break;
			}
		}
	}
}
