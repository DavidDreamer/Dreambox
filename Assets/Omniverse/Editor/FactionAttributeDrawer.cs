using UnityEditor;
using UnityEngine;

namespace Omniverse.Editor
{
	[CustomPropertyDrawer(typeof(FactionAttribute))]
	public class FactionAttributeDrawer: PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.intValue = EditorGUI.Popup(position, property.intValue, GlobalSettingsEditor.GlobalSettings.Factions);
		}
	}
}
