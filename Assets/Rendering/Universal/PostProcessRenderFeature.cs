using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public abstract class PostProcessRenderFeature<T> : CustomRendererFeature<T> where T : ScriptableRenderPass
	{
		[field: SerializeField]
		private Shader Shader { get; set; }

		public Material Material { get; private set; }

		public override void Create()
		{
			Dispose(false);

			Material = CoreUtils.CreateEngineMaterial(Shader);

			base.Create();
		}

		protected override void Dispose(bool disposing)
		{
			CoreUtils.Destroy(Material);
		}

		protected override bool IsValid()
		{
			if (Shader == null)
			{
				return false;
			}

			return true;
		}
	}
}
