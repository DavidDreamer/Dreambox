using System;
using UnityEngine;

namespace Dreambox.Rendering.Core
{
	[Serializable]
	public class BlurSettings
	{
		[field: SerializeField]
		public BlurAlgorithm Algorithm { get; set; }

		[field: SerializeField]
		[field: Range(1, 4)]
		public int Downsample { get; set; } = 1;

		[field: SerializeField]
		[field: Range(1, 100)]
		public int Radius { get; set; }
	}
}
