using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dreambox.Core.Editor
{
	public static class SerializedPropertyUtils
	{
		public static void DrawIcon(this SerializedProperty serializedProperty)
		{
			const float size = 64;
			EditorGUILayout.LabelField(serializedProperty.displayName);
			serializedProperty.objectReferenceValue =
				EditorGUILayout.ObjectField(serializedProperty.objectReferenceValue, typeof(Texture2D), false, GUILayout.Width(size), GUILayout.Height(size));
		}

		public static void DrawManagedReference(this SerializedProperty serializedProperty, Type type)
		{
			bool hasValue = serializedProperty.managedReferenceValue is not null;

			using (new GUILayout.VerticalScope())
			{
				using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
				{
					using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
					{
						hasValue = EditorGUILayout.Toggle(hasValue, GUILayout.Width(12));

						if (changeCheckScope.changed)
						{
							serializedProperty.managedReferenceValue = hasValue ? Activator.CreateInstance(type) : null;
						}
					}

					using (new EditorGUI.DisabledGroupScope(!hasValue))
					{
						EditorGUILayout.LabelField(serializedProperty.displayName, EditorStyles.boldLabel);
					}

					GUILayout.FlexibleSpace();
				}

				if (hasValue)
				{
					serializedProperty.DrawChildren();
				}
			}
		}

		public static void DrawManagedReferenceFromInterface(this SerializedProperty serializedProperty, Type type)
		{
			var possibleTypes = type.GetInheritedTypes();

			bool hasValue = serializedProperty.managedReferenceValue is not null;

			using (new GUILayout.VerticalScope())
			{
				using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
				{
					using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
					{
						hasValue = EditorGUILayout.Toggle(hasValue, GUILayout.Width(12));

						if (changeCheckScope.changed)
						{
							serializedProperty.managedReferenceValue =
								hasValue ? Activator.CreateInstance(possibleTypes.First()) : null;
						}
					}

					using (new EditorGUI.DisabledGroupScope(!hasValue))
					{
						EditorGUILayout.LabelField(serializedProperty.displayName, EditorStyles.boldLabel);
					}

					GUILayout.FlexibleSpace();
				}

				if (hasValue)
				{
					int index = possibleTypes.IndexOf(serializedProperty.managedReferenceValue.GetType());

					using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
					{
						string suffixToRemove = type.Name.Remove(0, 1);
						string[] options = possibleTypes.Select(t => t.Name.Replace(suffixToRemove, string.Empty))
							.ToArray();
						index = EditorGUILayout.Popup("Type", index, options);

						if (changeCheckScope.changed)
						{
							serializedProperty.managedReferenceValue = Activator.CreateInstance(possibleTypes[index]);
						}
					}

					serializedProperty.DrawChildren();
				}
			}
		}

		public static void DrawChildren(this SerializedProperty serializedProperty)
		{
			IEnumerator enumerator = serializedProperty.Copy().GetEnumerator();
			int depth = serializedProperty.depth + 1;

			while (enumerator.MoveNext())
			{
				var property = (SerializedProperty)enumerator.Current;
				if (property.depth != depth)
				{
					continue;
				}
				EditorGUILayout.PropertyField(property);
			}
		}
	}
}
