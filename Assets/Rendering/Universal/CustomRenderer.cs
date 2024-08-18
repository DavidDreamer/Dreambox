using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public abstract class CustomRenderer<TConfig, TPass> : MonoBehaviour
		where TConfig : CustomRendererConfig
		where TPass : ScriptableRenderPass, IDisposable
	{
		[field: SerializeField]
		public TConfig Config { get; private set; }

		private TPass Pass { get; set; }

		protected abstract TPass CreatePass();

		private void OnEnable()
		{
			Pass = CreatePass();
			Pass.renderPassEvent = Config.RenderPassEvent;

			RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
		}

		private void OnDisable()
		{
			RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
		}

		private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
		{
			if (IsInactive())
			{
				return;
			}

			if (Config.CameraType.HasFlag(cam.cameraType))
			{
				cam.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(Pass);

			}
		}

		protected abstract bool IsInactive();
	}
}

