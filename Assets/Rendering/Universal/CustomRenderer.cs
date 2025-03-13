using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public abstract class CustomRenderer<TConfig, TPass> : MonoBehaviour
		where TConfig : CustomRendererConfig
		where TPass : ScriptableRenderPass
	{
		[field: SerializeField]
		public TConfig Config { get; private set; }

		private TPass Pass { get; set; }

		private void OnEnable()
		{
			RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;

			Pass = Setup();
			Pass.renderPassEvent = Config.RenderPassEvent;
		}

		private void OnDisable()
		{
			RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;

			Cleanup();
		}

		protected abstract TPass Setup();

		protected virtual void Cleanup()
		{
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

