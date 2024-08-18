using System;
using UnityEditor;
using UnityEngine;

namespace Dreambox.Core.Editor
{
	[CustomPropertyDrawer(typeof(OptionalAttribute))]
	public class OptionalAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool hasValue = property.managedReferenceValue != null;

			using (new EditorGUI.IndentLevelScope(hasValue ? 1 : 0))
			using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
			{
				var togglePosition = new Rect(position)
				{
					height = EditorGUIUtility.singleLineHeight
				};

				bool toggle = EditorGUI.ToggleLeft(togglePosition, label, hasValue);

				if (changeCheckScope.changed)
				{
					if (toggle)
					{
						if (property.managedReferenceValue is null)
						{
							Type type = TypeUtils.GetTypeFromFullName(property.managedReferenceFieldTypename);
							property.managedReferenceValue = Activator.CreateInstance(type);
						}
					}
					else
					{
						property.managedReferenceValue = null;
					}
				}
			}

			if (hasValue)
			{
				EditorGUI.PropertyField(position, property, GUIContent.none, true);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, true);
	}
}
