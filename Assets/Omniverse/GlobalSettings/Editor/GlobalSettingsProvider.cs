using UnityEditor;
using UnityEngine.UIElements;

namespace Omniverse.Editor
{
	public class GlobalSettingsProvider: SettingsProvider
	{
		private const string Path = "Project/Omniverse";

		private SerializedObject SerializedObject { get; set; }

		private UnityEditor.Editor Editor { get; set; }

		[SettingsProvider]
		public static SettingsProvider Create() => new GlobalSettingsProvider();

		private GlobalSettingsProvider(): base(Path, SettingsScope.Project)
		{
		}

		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			SerializedObject = new SerializedObject(GlobalSettings.Instance);
			Editor = UnityEditor.Editor.CreateEditor(SerializedObject.targetObject);
		}

		public override void OnGUI(string searchContext)
		{
			SerializedObject.UpdateIfRequiredOrScript();
			Editor.OnInspectorGUI();
			SerializedObject.ApplyModifiedProperties();
		}
	}
}
