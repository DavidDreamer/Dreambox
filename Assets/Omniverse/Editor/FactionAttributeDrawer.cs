using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Omniverse.Editor
{
	[CustomPropertyDrawer(typeof(FactionAttribute))]
	public class FactionAttributeDrawer: PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			string[] factionNames =
				GlobalSettings.Instance.Factions.Select(faction => faction.Name).ToArray();
			
			property.intValue = EditorGUI.Popup(position, property.intValue, factionNames);
		}
	}
}
