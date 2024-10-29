using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dreambox.Core.Editor
{
	public static class VersatileSerializedPropertyUtils
	{
		public static void OptionalVersatileField(this SerializedProperty serializedProperty, Type type, bool includeChildren = true)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				bool hasValue = serializedProperty.managedReferenceValue is not null;
				bool shouldHaveValue = EditorGUILayout.ToggleLeft(serializedProperty.displayName, hasValue);

				if (shouldHaveValue)
				{
					serializedProperty.VersatileField(type, GUIContent.none, false);
				}
				else
				{
					serializedProperty.managedReferenceValue = null;
				}
			}

			if (includeChildren)
			{
				serializedProperty.DrawChildrenIndented();
			}
		}

		public static void VersatileField(this SerializedProperty serializedProperty, Type type, bool includeChildren = true)
			=> serializedProperty.VersatileField(type, new GUIContent(serializedProperty.displayName), includeChildren);
	
		public static void VersatileField(this SerializedProperty serializedProperty, Type type, GUIContent label, bool includeChildren = true)
		{
			var inheritedTypes = type.GetInheritedTypes();

			if (inheritedTypes.Count == 0)
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel(serializedProperty.displayName);
					EditorGUILayout.HelpBox("No matching types found.", MessageType.None);
				}

				serializedProperty.managedReferenceValue = null;

				return;
			}

			var displayedOptions = inheritedTypes.Select(t => t.ToGUIContent(type)).ToArray();

			int selectedIndex = serializedProperty.managedReferenceValue is null
				? 0
				: inheritedTypes.IndexOf(serializedProperty.managedReferenceValue.GetType());

			selectedIndex = EditorGUILayout.Popup(label, selectedIndex, displayedOptions);

			Type selectedType = inheritedTypes[selectedIndex];

			if (serializedProperty.managedReferenceValue is null || serializedProperty.managedReferenceValue.GetType() != selectedType)
			{
				serializedProperty.managedReferenceValue = Activator.CreateInstance(selectedType);
			}

			if (includeChildren)
			{
				serializedProperty.DrawChildrenIndented();
			}
		}

		public static GUIContent ToGUIContent(this Type type, Type parentType)
		{
			string parentTypeName = parentType.Name;
			if (parentTypeName.StartsWith("I") && parentTypeName.Length > 1)
			{
				parentTypeName = parentTypeName[1..];
			}

			int indexOfGenericParameter = parentTypeName.IndexOf('`');
			if (indexOfGenericParameter != -1)
			{
				parentTypeName = parentTypeName[..indexOfGenericParameter];
			}

			string typeName = type.Name;
			typeName = typeName.Replace(parentTypeName, string.Empty);

			typeName = typeName.SplitCamelCaseWithSpaces();

			return new GUIContent(typeName);
		}
	}
}
