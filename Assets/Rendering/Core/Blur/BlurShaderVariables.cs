using UnityEngine;

namespace Dreambox.Rendering.Core
{
	public static class BlurShaderVariables
	{
		public static string ToKeywordName(this BlurAlgorithm algorithm) =>
			$"ALGORITHM_{algorithm.ToString().ToUpper()}";

		public static string ToKeywordName(this WrapMode wrapMode) => $"WRAP_MODE_{wrapMode.ToString().ToUpper()}";

		public static int Radius { get; } = Shader.PropertyToID(nameof(Radius));

		public static int Factor { get; } = Shader.PropertyToID(nameof(Factor));
	}
}
