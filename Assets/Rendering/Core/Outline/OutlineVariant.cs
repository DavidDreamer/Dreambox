using System;
using UnityEngine;

namespace Dreambox.Rendering.URP
{
	[Serializable]
	public struct OutlineVariant
	{
		public Color Color;
		
		[Range(0f, 1f)]
		public float Width;

		[Range(1f, 10f)]
		public float Softness;

		public Color FillColor;

		public Color FillFlickColor;

		public float FillFlickRate;
	}
}
