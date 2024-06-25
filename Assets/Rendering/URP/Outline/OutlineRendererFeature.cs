using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.URP
{
	public class OutlineRendererFeature: ScriptableRendererFeature
	{
		[field: SerializeField]
		private RenderPassEvent RenderPassEvent { get; set; }

		[field: SerializeField]
		private OutlineConfig Config { get; set; }

		public OutlinePass Pass { get; private set; }

		public override void Create()
		{
			Pass?.Dispose();

			Pass = new OutlinePass(Config)
			{
				renderPassEvent = RenderPassEvent
			};
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			Pass?.Dispose();
			Pass = null;
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			CameraData cameraData = renderingData.cameraData;
			if (cameraData.cameraType is CameraType.Game or CameraType.SceneView)
			{
				renderer.EnqueuePass(Pass);
			}
		}
	}
}
