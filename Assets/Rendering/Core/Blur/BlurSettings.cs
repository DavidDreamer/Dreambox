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
		public WrapMode WrapMode { get; set; }
		
		[field: SerializeField]
		[field: Range(1, 4)]
		public int Downsample { get; set; } = 1;

		[field: SerializeField]
		[field: Range(1, 100)]
		public int Radius { get; set; }

		[field: SerializeField]
		[field: Range(0f, 1f)]
		public float Factor { get; set; }
		
		public void ApplyTo(Material material)
		{
			material.EnableKeyword(Algorithm.ToKeywordName());
			material.EnableKeyword(WrapMode.ToKeywordName());
			material.SetInt(BlurShaderVariables.Radius, Radius);
			material.SetFloat(BlurShaderVariables.Factor, Factor);
		}
	}
}
