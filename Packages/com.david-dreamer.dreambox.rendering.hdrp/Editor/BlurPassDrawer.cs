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
        private SerializedProperty Multiplier { get; set; }
        private SerializedProperty Sigma { get; set; }
        private SerializedProperty Target { get; set; }

        private BlurPass Pass => (BlurPass)target;

        protected override void Initialize(SerializedProperty customPass)
        {
            base.Initialize(customPass);

            Downsample = customPass.FindPropertyRelative(nameof(BlurPass.Downsample).ToBackingField());
            KernelSize = customPass.FindPropertyRelative(nameof(BlurPass.KernelSize).ToBackingField());
            Multiplier = customPass.FindPropertyRelative(nameof(BlurPass.Multiplier).ToBackingField());
            Sigma = customPass.FindPropertyRelative(nameof(BlurPass.Sigma).ToBackingField());
            Target = customPass.FindPropertyRelative(nameof(BlurPass.Target).ToBackingField());
        }

        protected override void DoPassGUI(SerializedProperty customPassProp, Rect rect)
        {
            using var changeScope = new EditorGUI.ChangeCheckScope();

            EditorGUILayout.PropertyField(Downsample);
            EditorGUILayout.PropertyField(KernelSize);
            EditorGUILayout.PropertyField(Multiplier);
            EditorGUILayout.PropertyField(Sigma);
            EditorGUILayout.PropertyField(Target);

            if (changeScope.changed)
            {
                Pass.Reset();
            }
        }
    }
}
