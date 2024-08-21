using UnityEngine;

namespace Dreambox.Rendering.Universal
{
	public struct OutlineTarget
	{
		public Renderer Renderer { get; }

		public int Variant { get; }

		public OutlineTarget(Renderer renderer, int variant)
		{
			Renderer = renderer;
			Variant = variant;
		}
	}
}
