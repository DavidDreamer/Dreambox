using UnityEngine;

namespace Dreambox.Rendering.Core
{
	public static class OutlineShaderVariable
	{
		public static int StepWidth { get; } = Shader.PropertyToID(nameof(StepWidth));

		public static int VariantsBuffer { get; } = Shader.PropertyToID(nameof(VariantsBuffer));

		public static int Variant { get; } = Shader.PropertyToID(nameof(Variant));

		public static int BaseMap { get; } = Shader.PropertyToID($"_{nameof(BaseMap)}");
	}
}
