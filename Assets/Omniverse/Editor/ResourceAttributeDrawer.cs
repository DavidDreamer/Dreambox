using UnityEditor;
using UnityEngine;

namespace Omniverse.Editor
{
	[CustomPropertyDrawer(typeof(ResourceAttribute))]
	public class ResourceAttributeDrawer: PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.intValue = EditorGUI.Popup(position, property.intValue, GeneralSettingsEditor.GeneralSettings.Resources);
		}
	}
}
