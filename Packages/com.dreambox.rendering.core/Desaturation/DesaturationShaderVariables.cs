using UnityEngine;

namespace Dreambox.Rendering.Core
{
	public static class DesaturationShaderVariables
	{
		public static int Factor { get; } = Shader.PropertyToID(nameof(Factor));
	}
}
