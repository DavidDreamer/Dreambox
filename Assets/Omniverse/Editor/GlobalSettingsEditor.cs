using System.Linq;
using UnityEditor;

namespace Omniverse.Editor
{
	public static class GlobalSettingsEditor
	{
		private static GlobalSettings _globalSettings;

		public static GlobalSettings GlobalSettings
		{
			get
			{
				if (_globalSettings is null)
				{
					string guid = AssetDatabase.FindAssets($"t:{typeof(GlobalSettings)}").First();
					string path = AssetDatabase.GUIDToAssetPath(guid);
					_globalSettings = AssetDatabase.LoadAssetAtPath<GlobalSettings>(path);
				}

				return _globalSettings;
			}
		}
	}
}
