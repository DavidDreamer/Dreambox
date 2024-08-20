using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VContainer;
using VContainer.Unity;

namespace Dreambox.Rendering.Universal
{
	public abstract class CustomRenderer<TConfig, TPass> : MonoBehaviour, IInitializable, IDisposable
		where TConfig : CustomRendererConfig
		where TPass : ScriptableRenderPass, IDisposable
	{
		[Inject]
		public TConfig Config { get; private set; }

		private TPass Pass { get; set; }

		protected abstract TPass CreatePass();

		public virtual void Initialize()
		{
			Pass = CreatePass();
			Pass.renderPassEvent = Config.RenderPassEvent;
		}

		public virtual void Dispose()
		{
			Pass.Dispose();
		}

		private void OnEnable()
		{
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

