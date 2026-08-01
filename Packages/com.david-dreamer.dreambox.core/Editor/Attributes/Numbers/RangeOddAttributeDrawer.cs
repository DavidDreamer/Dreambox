using UnityEditor;
using UnityEngine;
using Dreambox.Math;

namespace Dreambox.Core.Editor
{
	[CustomPropertyDrawer(typeof(RangeOddAttribute))]
	public class RangeOddAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.LabelField(position, label.text, "Error: Must be an int.");
				return;
			}

			EditorGUI.BeginChangeCheck();

			var oddRange = (RangeOddAttribute)attribute;

			int newValue = EditorGUI.IntSlider(position, label, property.intValue, oddRange.min, oddRange.max);

			if (EditorGUI.EndChangeCheck())
			{
				property.intValue = newValue.ClosestOdd();
			}
		}
	}
}
