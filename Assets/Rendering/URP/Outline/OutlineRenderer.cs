using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.URP
{
	public class OutlineRenderer: MonoBehaviour
	{
		[field: SerializeField]
		private RenderPassEvent RenderPassEvent { get; set; }

		[field: SerializeField]
		private OutlineConfig Config { get; set; }

		public OutlinePass Pass { get; private set; }

		private void Awake()
		{
			Pass = new OutlinePass(Config)
			{
				renderPassEvent = RenderPassEvent
			};
		}

		private void OnDestroy()
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
			if (cam.cameraType is CameraType.Game or CameraType.SceneView)
			{
				cam.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(Pass);
			}
		}
	}
}
