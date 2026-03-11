using UnityEditor;
using UnityEditor.UI;
using Dreambox.Core.Editor;

namespace Dreambox.UI.Editor
{
	[CustomEditor(typeof(AdvancedImage), true)]
	[CanEditMultipleObjects]
	public class AdvancedImageEditor : ImageEditor
	{
		private SerializedProperty ColorLeftBottom { get; set; }
		private SerializedProperty ColorLeftTop { get; set; }
		private SerializedProperty ColorRightBottom { get; set; }
		private SerializedProperty ColorRightTop { get; set; }

		protected override void OnEnable()
		{
			base.OnEnable();

			ColorLeftBottom = serializedObject.FindProperty(nameof(AdvancedImage.ColorLeftBottom).ToBackingField());
			ColorLeftTop = serializedObject.FindProperty(nameof(AdvancedImage.ColorLeftTop).ToBackingField());
			ColorRightBottom = serializedObject.FindProperty(nameof(AdvancedImage.ColorRightBottom).ToBackingField());
			ColorRightTop = serializedObject.FindProperty(nameof(AdvancedImage.ColorRightTop).ToBackingField());
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.PropertyField(ColorLeftBottom);
			EditorGUILayout.PropertyField(ColorLeftTop);
			EditorGUILayout.PropertyField(ColorRightBottom);
			EditorGUILayout.PropertyField(ColorRightTop);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
