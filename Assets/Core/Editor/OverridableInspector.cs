// Copyright (c) Saber BGS 2023. All rights reserved.
// ---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Dreambox.Core.Editor
{
	public abstract class OverridableInspector: UnityEditor.Editor
	{
		private const string ScriptName = "m_Script";

		protected abstract IEnumerable<string> CustomProperties { get; }
		
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			
			using (SerializedProperty iterator = serializedObject.GetIterator())
			{
				bool expanded = true;
				while (iterator.NextVisible(expanded))
				{
					expanded = false;

					if (iterator.name == ScriptName)
					{
						using (new EditorGUI.DisabledScope(true))
						{
							EditorGUILayout.PropertyField(iterator, true);
						}
						
						continue;
					}
					
					if (CustomProperties.Contains(iterator.name))
					{
						DrawCustomProperty(iterator);
					}
					else
					{
						EditorGUILayout.PropertyField(iterator, true);
					}
				}
			}

			DrawAdditionalOptions();
			
			serializedObject.ApplyModifiedProperties();
		}

		protected abstract void DrawCustomProperty(SerializedProperty property);
		
		protected virtual void DrawAdditionalOptions()
		{
		}
	}
}
