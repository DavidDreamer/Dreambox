namespace Dreambox.Rendering.Core
{
	public static class BlurShaderVariable
	{
		public static int Radius { get; } = ShaderVariable.Create();
		public static int Multiplier { get; } = ShaderVariable.Create();
		public static int Kernel { get; } = ShaderVariable.Create();
		public static int BlurTexture { get; } = ShaderVariable.Create();
	}
}
