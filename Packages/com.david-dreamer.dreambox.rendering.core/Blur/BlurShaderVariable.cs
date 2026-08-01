namespace Dreambox.Rendering.Core
{
	public static class BlurShaderVariable
	{
		public static int Radius { get; } = ShaderVariable.Create();
		public static int Scale { get; } = ShaderVariable.Create();
		public static int Kernel { get; } = ShaderVariable.Create();
		public static int BlurTexture { get; } = ShaderVariable.Create();
	}
}
