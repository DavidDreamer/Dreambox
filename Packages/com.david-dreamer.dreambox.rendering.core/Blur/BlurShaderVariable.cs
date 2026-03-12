namespace Dreambox.Rendering.Core
{
	public static class BlurShaderVariable
	{
		public static int Radius { get; } = ShaderVariable.Create();
		public static int GaussianWeights { get; } = ShaderVariable.Create();
	}
}
