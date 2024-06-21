using System;
using UnityEngine;

namespace Dreambox.Rendering.URP
{
	[Serializable]
	public struct OutlineVariant
	{
		public Color OutlineColor;

		public Color FillColor;

		[Range(0f, 0.05f)]
		public float Width;

		[Range(0f, 1f)]
		public float Softness;

		[Range(1f, 5f)]
		public float SoftnessPower;

		[Range(0f, 1f)]
		public float PixelOffset;

		public Color FillFlickColor;

		public float FillFlickRate;

		[Range(0f, 0.05f)]
		public float CutOffWidth;
	}
}
