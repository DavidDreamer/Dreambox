using UnityEngine;

namespace Dreambox.Rendering.Universal
{
	public struct OutlineRenderer
	{
		public Renderer Renderer { get; }

		public int Variant { get; }

		public OutlineRenderer(Renderer renderer, int variant)
		{
			Renderer = renderer;
			Variant = variant;
		}
	}
}
