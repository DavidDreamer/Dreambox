using System.Linq;
using UnityEditor;

namespace Omniverse.Editor
{
	public static class GeneralSettingsEditor
	{
		private static GeneralSettings _generalSettings;

		public static GeneralSettings GeneralSettings
		{
			get
			{
				if (_generalSettings is null)
				{
					string guid = AssetDatabase.FindAssets($"t:{typeof(GeneralSettings)}").First();
					string path = AssetDatabase.GUIDToAssetPath(guid);
					_generalSettings = AssetDatabase.LoadAssetAtPath<GeneralSettings>(path);
				}

				return _generalSettings;
			}
		}
	}
}
