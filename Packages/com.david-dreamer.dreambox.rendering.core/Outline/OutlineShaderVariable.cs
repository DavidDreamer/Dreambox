using UnityEngine;

namespace Dreambox.Rendering.Core
{
	public static class OutlineShaderVariable
	{
		public static int MaskTexture { get; } = ShaderVariable.Create();

		public static int JumpFloodSourceTexture { get; } = ShaderVariable.Create();

		public static int JumpFloodTargetTexture { get; } = ShaderVariable.Create();

		public static int TextureResolution { get; } = ShaderVariable.Create();

		public static int StepWidth { get; } = ShaderVariable.Create();

		public static int VariantsBuffer { get; } = ShaderVariable.Create();

		public static int Variant { get; } = ShaderVariable.Create();

		public static int BaseMap { get; } = ShaderVariable.Create();
	}
}
