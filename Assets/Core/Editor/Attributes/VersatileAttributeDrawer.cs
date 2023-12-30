using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dreambox.Core.Editor
{
	[CustomPropertyDrawer(typeof(VersatileAttribute))]
	public class VersatileAttributeDrawer: PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var versatileTypeAttribute = (VersatileAttribute)attribute;
			var inheritedTypes = versatileTypeAttribute.Type.GetInheritedTypes();

			var displayedOptions =
				inheritedTypes.Select(type => new GUIContent(type.Name.SplitCamelCaseWithSpaces())).ToArray();

			int selectedOption = property.managedReferenceValue is null
				? 0
				: inheritedTypes.IndexOf(property.managedReferenceValue.GetType());
		
			var selectorPosition = new Rect(position)
			{
				height = EditorGUIUtility.singleLineHeight
			};

			selectedOption = EditorGUI.Popup(selectorPosition, label, selectedOption, displayedOptions);

			Type selectedType = inheritedTypes[selectedOption];

			if (property.managedReferenceValue is null || property.managedReferenceValue.GetType() != selectedType)
			{
				property.managedReferenceValue = Activator.CreateInstance(selectedType);
			}
			
			EditorGUI.PropertyField(position, property, GUIContent.none, true);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, true);
	}
}
