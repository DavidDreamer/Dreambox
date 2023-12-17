using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Omniverse.Editor
{
	[CustomPropertyDrawer(typeof(ResourceNameAttribute))]
	public class ResourceNameDrawer: PropertyDrawer
	{
		private static GlobalSettings _globalSettings;

		private static GlobalSettings GlobalSettings
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

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.intValue = EditorGUI.Popup(position, property.intValue, GlobalSettings.Resources);
		}
	}
}
