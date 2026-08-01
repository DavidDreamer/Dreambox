using UnityEditor;
using UnityEditor.Rendering.HighDefinition;
using UnityEngine;

namespace Dreambox.Rendering.HDRP
{
    [CustomPassDrawer(typeof(BlurPass))]
    public class BlurPassDrawer : CustomPassDrawer
    {
        private BlurPass Pass => (BlurPass)target;

        protected override PassUIFlag commonPassUIFlags => PassUIFlag.None;

        protected override void DoPassGUI(SerializedProperty customPassProp, Rect rect)
        {
            using var changeScope = new EditorGUI.ChangeCheckScope();

            base.DoPassGUI(customPassProp, rect);

            customPassProp.serializedObject.ApplyModifiedProperties();

            if (changeScope.changed)
            {
                Pass.Reset();
            }
        }
    }
}
