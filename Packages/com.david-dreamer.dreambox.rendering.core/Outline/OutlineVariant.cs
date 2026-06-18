using System;
using UnityEngine;

namespace Dreambox.Rendering.Core
{
	[Serializable]
	public struct OutlineVariant
	{
		public Color Color;

		[Range(1, 1000)]
		public int Width;

		[Range(1f, 10f)]
		public float Softness;
	}
}
