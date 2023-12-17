using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Omniverse.Editor
{
	public class EntityEditor: EditorWindow
	{
		private enum TabType
		{
			Units,

			Abilities
		}

		[MenuItem("Dreambox/EntityEditor")]
		private static void CreateWindow()
		{
			GetWindow<EntityEditor>(nameof(EntityEditor));
		}

		private TabType SelectedTabType { get; set; }

		private List<string> Entities { get; set; }

		private int SelectedIndex { get; set; }

		private Object SelectedAsset { get; set; }

		private UnityEditor.Editor SelectedEntityEditor { get; set; }

		protected virtual void OnGUI()
		{
			DrawTabs();
			DrawEntities();
		}

		private void DrawTabs()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				foreach (TabType tabType in Enum.GetValues(typeof(TabType)).Cast<TabType>())
				{
					if (GUILayout.Toggle(SelectedTabType == tabType, tabType.ToString(), EditorStyles.toolbarButton))
					{
						SelectTab(tabType);
					}
				}

				void SelectTab(TabType tabType)
				{
					SelectedTabType = tabType;

					switch (SelectedTabType)
					{
						case TabType.Units:
							UpdateUnits();
							break;
						case TabType.Abilities:
							UpdateAbilities();
							break;
					}
				}

				void UpdateUnits()
				{
					Entities = AssetDatabase.FindAssets("t:Unit").Select(AssetDatabase.GUIDToAssetPath).ToList();
				}

				void UpdateAbilities()
				{
					Entities = AssetDatabase.FindAssets("t:Ability").Select(AssetDatabase.GUIDToAssetPath).ToList();
				}
			}
		}

		private void DrawEntities()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				if (Entities.Count is 0)
				{
					EditorGUILayout.HelpBox("None.", MessageType.Info);
				}
				else
				{
					DrawEntitiesList();
					DrawSelectedEntity();
				}
			}

			void DrawEntitiesList()
			{
				using (new EditorGUILayout.VerticalScope())
				{
					for (int i = 0; i < Entities.Count; ++i)
					{
						string assetPath = Entities[i];

						string label = assetPath[(assetPath.LastIndexOf("/", StringComparison.Ordinal) + 1)..];
						label = label.Remove(label.LastIndexOf(".asset", StringComparison.Ordinal));

						if (GUILayout.Toggle(i == SelectedIndex, label, EditorStyles.toolbarButton))
						{
							SelectedIndex = i;
							SelectedAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
							SelectedEntityEditor = UnityEditor.Editor.CreateEditor(SelectedAsset);
						}
					}
				}
			}

			void DrawSelectedEntity()
			{
				using (new EditorGUILayout.VerticalScope())
				{
					SelectedEntityEditor.OnInspectorGUI();
				}
			}
		}
	}
}
