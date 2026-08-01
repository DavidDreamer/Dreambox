using UnityEditor;
using UnityEngine;
using Dreambox.Math;

namespace Dreambox.Core.Editor
{
	[CustomPropertyDrawer(typeof(RangeEvenAttribute))]
	public class RangeEvenAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.LabelField(position, label.text, "Error: Must be an int.");
				return;
			}

			EditorGUI.BeginChangeCheck();

			var evenRange = (RangeEvenAttribute)attribute;

			int newValue = EditorGUI.IntSlider(position, label, property.intValue, evenRange.min, evenRange.max);

			if (EditorGUI.EndChangeCheck())
			{
				property.intValue = newValue.ClosestEven();
			}
		}
	}
}
