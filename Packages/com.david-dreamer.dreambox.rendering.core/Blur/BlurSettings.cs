using System;
using Dreambox.Core;
using UnityEngine;

namespace Dreambox.Rendering.Core
{
	[Serializable]
	public class BlurSettings
	{
		[field: Range(1, 8)]
		public int Downsample = 2;

		[field: RangeOdd(3, 51)]
		public int KernelSize = 15;

		[field: Range(1, 10)]
		public float Scale = 1;

		[field: Range(1f, 10f)]
		public float Sigma = 3f;
	}
}
