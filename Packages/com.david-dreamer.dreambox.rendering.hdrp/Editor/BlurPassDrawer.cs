using Dreambox.Core.Editor;
using UnityEditor;
using UnityEditor.Rendering.HighDefinition;
using UnityEngine;

namespace Dreambox.Rendering.HDRP
{
    [CustomPassDrawer(typeof(BlurPass))]
    public class BlurPassDrawer : CustomPassDrawer
    {
        private SerializedProperty Downsample { get; set; }
        private SerializedProperty KernelSize { get; set; }
        private SerializedProperty Scale { get; set; }
        private SerializedProperty Sigma { get; set; }
        private SerializedProperty Target { get; set; }

        private BlurPass Pass => (BlurPass)target;

        protected override void Initialize(SerializedProperty customPass)
        {
            base.Initialize(customPass);

            Downsample = customPass.FindPropertyRelative(nameof(BlurPass.Downsample).ToBackingField());
            KernelSize = customPass.FindPropertyRelative(nameof(BlurPass.KernelSize).ToBackingField());
            Scale = customPass.FindPropertyRelative(nameof(BlurPass.Scale).ToBackingField());
            Sigma = customPass.FindPropertyRelative(nameof(BlurPass.Sigma).ToBackingField());
            Target = customPass.FindPropertyRelative(nameof(BlurPass.Target).ToBackingField());
        }

        protected override void DoPassGUI(SerializedProperty customPassProp, Rect rect)
        {
            customPassProp.serializedObject.Update();

            using var changeScope = new EditorGUI.ChangeCheckScope();

            EditorGUILayout.PropertyField(Downsample);

            EditorGUILayout.PropertyField(KernelSize);
            if (KernelSize.intValue % 2 == 0)
            {
                KernelSize.intValue += 1;
            }

            EditorGUILayout.PropertyField(Scale);
            EditorGUILayout.PropertyField(Sigma);
            EditorGUILayout.PropertyField(Target);

            customPassProp.serializedObject.ApplyModifiedProperties();

            if (changeScope.changed)
            {
                Pass.Reset();
            }
        }
    }
}
