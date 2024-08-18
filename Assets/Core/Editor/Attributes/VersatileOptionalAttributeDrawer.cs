using UnityEditor;
using UnityEngine;

namespace Dreambox.Core.Editor
{
	[CustomPropertyDrawer(typeof(VersatileOptionalAttribute))]
	public class VersatileOptionalAttributeDrawer : VersatileAttributeDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool hasValue = property.managedReferenceValue != null;

			using (new EditorGUI.IndentLevelScope(hasValue ? 1 : 0))
			{
				var togglePosition = new Rect(position)
				{
					height = EditorGUIUtility.singleLineHeight,
					width = position.width / 2f
				};

				hasValue = EditorGUI.ToggleLeft(togglePosition, label, hasValue);
			}

			if (hasValue)
			{
				base.OnGUI(position, property, GUIContent.none);
			}
			else
			{
				property.managedReferenceValue = null;
			}
		}
	}
}
