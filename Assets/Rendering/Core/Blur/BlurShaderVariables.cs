using System;
using UnityEngine;

namespace Dreambox.Rendering.Core
{
	public static class BlurShaderVariables
	{
		public static string ToKeywordName(this BlurAlgorithm algorithm)
		{
			switch (algorithm)
			{
				case BlurAlgorithm.Box:
					return "ALGORITHM_BOX";
				case BlurAlgorithm.Gaussian:
					return "ALGORITHM_GAUSSIAN";
				default:
					throw new ArgumentOutOfRangeException(nameof(algorithm));
			}
		}
		
		public static int Radius { get; } = Shader.PropertyToID(nameof(Radius));
		public static int Factor { get; } = Shader.PropertyToID(nameof(Factor));
	}
}
