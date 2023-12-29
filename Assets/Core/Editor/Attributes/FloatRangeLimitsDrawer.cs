using UnityEditor;
using UnityEngine;

namespace Dreambox.Core.Editor
{
	[CustomPropertyDrawer(typeof(FloatRangeLimitsAttribute))]
	public class FloatRangeLimitsDrawer: PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var limits = (FloatRangeLimitsAttribute)attribute;

			SerializedProperty min = property.FindPropertyRelative(nameof(FloatRange.Min).ToBackingField());
			SerializedProperty max = property.FindPropertyRelative(nameof(FloatRange.Max).ToBackingField());

			float minValue = min.floatValue;
			float maxValue = max.floatValue;

			position = EditorGUI.PrefixLabel(position, label);

			 Rect minValuePosition = position;
			 minValuePosition.width = EditorGUIUtility.fieldWidth;
			 position.width -= EditorGUIUtility.fieldWidth * 2f;
			 position.x += EditorGUIUtility.fieldWidth;
			
			 minValue = EditorGUI.FloatField(minValuePosition, minValue);
			 minValue = Mathf.Clamp(minValue, limits.Min, maxValue);
			
			 EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, limits.Min, limits.Max);
			
			 position.x += position.width;
			
			 maxValue = EditorGUI.FloatField(position, maxValue);
			 maxValue = Mathf.Clamp(maxValue, minValue, limits.Max);
			
			 minValue = Mathf.Round(minValue * 10f) / 10f;
			 maxValue = Mathf.Round(maxValue * 10f) / 10f;
			
			 min.floatValue = minValue;
			 max.floatValue = maxValue;
		}
	}
}
