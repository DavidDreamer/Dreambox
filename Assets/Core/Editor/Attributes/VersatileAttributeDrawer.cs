﻿using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dreambox.Core.Editor
{
	[CustomPropertyDrawer(typeof(VersatileAttribute))]
	public class VersatileAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var versatileTypeAttribute = (VersatileAttribute)attribute;

			Type parentType = versatileTypeAttribute.Type;

			var inheritedTypes = versatileTypeAttribute.Type.GetInheritedTypes();

			var displayedOptions = inheritedTypes.Select(type => type.ToGUIContent(parentType)).ToArray();

			int selectedOption = property.managedReferenceValue is null
				? 0
				: inheritedTypes.IndexOf(property.managedReferenceValue.GetType());

			var popupPosition = EditorGUI.PrefixLabel(position, label);

			if (inheritedTypes.Count == 0)
			{
				EditorGUI.HelpBox(popupPosition, "No matching types found.", MessageType.Error);
			}
			else
			{
				selectedOption = EditorGUI.Popup(popupPosition, GUIContent.none, selectedOption, displayedOptions);

				Type selectedType = inheritedTypes[selectedOption];

				if (property.managedReferenceValue is null || property.managedReferenceValue.GetType() != selectedType)
				{
					property.managedReferenceValue = Activator.CreateInstance(selectedType);
				}

				EditorGUI.PropertyField(position, property, GUIContent.none, true);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, true);
	}
}
