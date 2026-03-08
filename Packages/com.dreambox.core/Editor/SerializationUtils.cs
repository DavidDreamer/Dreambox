using UnityEditor;
using UnityEngine;

namespace Dreambox.Core.Editor
{
	public static class SerializationUtils
	{
		public static string ToBackingField(this string propertyName) => $"<{propertyName}>k__BackingField";

		public static void DrawArrayToolbar(this SerializedProperty serializedProperty)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Add"))
				{
					serializedProperty.InsertArrayElementAtIndex(serializedProperty.arraySize);
				}

				using (new EditorGUI.DisabledScope(serializedProperty.arraySize == 0))
				{
					if (GUILayout.Button("Remove"))
					{
						serializedProperty.arraySize--;
					}
				}
			}
		}
	}
}
