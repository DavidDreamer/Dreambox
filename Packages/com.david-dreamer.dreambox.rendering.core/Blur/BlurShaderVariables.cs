using UnityEngine;

namespace Dreambox.Rendering.Core
{
	public static class BlurShaderVariables
	{
		public static int Radius { get; } = Shader.PropertyToID(nameof(Radius));

		public static int Factor { get; } = Shader.PropertyToID(nameof(Factor));
	}
}
