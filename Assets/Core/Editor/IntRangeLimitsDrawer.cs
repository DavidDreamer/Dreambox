using UnityEditor;
using UnityEngine;

namespace Dreambox.Editor
{
	[CustomPropertyDrawer(typeof(IntRangeLimitsAttribute))]
	public class IntRangeLimitsDrawer: PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var limits = (IntRangeLimitsAttribute)attribute;

			SerializedProperty min = property.FindPropertyRelative(nameof(FloatRange.Min).ToBackingField());
			SerializedProperty max = property.FindPropertyRelative(nameof(FloatRange.Max).ToBackingField());

			int minValue = min.intValue;
			int maxValue = max.intValue;
			
			position = EditorGUI.PrefixLabel(position, label);

			Rect minValuePosition = position;
			minValuePosition.width = EditorGUIUtility.fieldWidth;
			position.width -= EditorGUIUtility.fieldWidth * 2f;
			position.x += EditorGUIUtility.fieldWidth;

			minValue = EditorGUI.IntField(minValuePosition, minValue);
			minValue = Mathf.Clamp(minValue, limits.Min, maxValue);

			float minValueFloat = minValue;
			float maxValueFloat = maxValue;
			
			EditorGUI.MinMaxSlider(position, ref minValueFloat, ref maxValueFloat, limits.Min, limits.Max);

			minValue = (int)minValueFloat;
			maxValue = (int)maxValueFloat;
			
			position.x += position.width;
			position.width = EditorGUIUtility.fieldWidth;
			
			maxValue = EditorGUI.IntField(position, maxValue);
			maxValue = Mathf.Clamp(maxValue, minValue, limits.Max);
	
			min.intValue = minValue;
			max.intValue = maxValue;
		}
	}
}
