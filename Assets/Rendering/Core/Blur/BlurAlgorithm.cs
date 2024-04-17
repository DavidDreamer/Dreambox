using System;

namespace Dreambox.Rendering.Core
{
	public enum BlurAlgorithm
	{
		Box,

		Gaussian
	}

	public static class AlgorithmUtils
	{
		public static string ToShaderName(this BlurAlgorithm algorithm)
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
	}
}
