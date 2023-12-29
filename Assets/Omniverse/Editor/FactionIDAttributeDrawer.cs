using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Omniverse.Editor
{
	[CustomPropertyDrawer(typeof(FactionIDAttribute))]
	public class FactionIDAttributeDrawer: PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			string[] factionNames =
				GeneralSettingsEditor.GeneralSettings.Factions.Select(faction => faction.Name).ToArray();
			
			property.intValue = EditorGUI.Popup(position, property.intValue, factionNames);
		}
	}
}
