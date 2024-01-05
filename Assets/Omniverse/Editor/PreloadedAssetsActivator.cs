using UnityEditor;

namespace Omniverse.Editor
{
	public static class PreloadedAssetsActivator
	{
		[InitializeOnLoadMethod]
		private static void Activate() => PlayerSettings.GetPreloadedAssets();
	}
}
