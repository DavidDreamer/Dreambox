using UnityEditor;
using UnityEngine;
using Dreambox.Math;

namespace Dreambox.Core.Editor
{
	[CustomPropertyDrawer(typeof(EvenAttribute))]
	public class EvenAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.LabelField(position, label.text, "Error: Must be an int.");
				return;
			}

			EditorGUI.BeginChangeCheck();

			int newValue = EditorGUI.IntField(position, label, property.intValue);

			if (EditorGUI.EndChangeCheck())
			{
				property.intValue = newValue.ClosestEven();
			}
		}
	}
}
