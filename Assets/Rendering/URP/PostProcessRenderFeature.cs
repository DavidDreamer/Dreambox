using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.URP
{
	public abstract class PostProcessRenderFeature<T>: ScriptableRendererFeature where T : ScriptableRenderPass, new()
	{
		[field: SerializeField]
		protected RenderPassEvent RenderPassEvent { get; set; }

		[field: SerializeField]
		private Shader Shader { get; set; }

		public Material Material { get; private set; }

		protected T Pass { get; set; }

		public override void Create()
		{
			Dispose(false);

			Material = CoreUtils.CreateEngineMaterial(Shader);

			Pass = new T { renderPassEvent = RenderPassEvent };
		}

		protected override void Dispose(bool disposing)
		{
			CoreUtils.Destroy(Material);
		}
	}
}
