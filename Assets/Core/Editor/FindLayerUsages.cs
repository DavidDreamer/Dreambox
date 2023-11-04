using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Dreambox.Editor
{
	public class FindLayerUsages: EditorWindow
	{
		private int Layer { get; set; }

		private int NewLayer { get; set; }

		private List<GameObject> Prefabs { get; } = new();

		private List<SceneAsset> Scenes { get; } = new();

		private Vector2 PrefabsListScrollPosition { get; set; }

		private Vector2 ScenesListScrollPosition { get; set; }

		[MenuItem("Dreambox/Find Layer Usages")]
		private static void CreateWindow()
		{
			GetWindow<FindLayerUsages>("Find Layer Usages");
		}

		protected virtual void OnGUI()
		{
			DrawLayers();

			using (new EditorGUILayout.HorizontalScope())
			{
				DrawPrefabs();
				DrawScenes();
			}
		}

		private void DrawLayers()
		{
			using (new EditorGUILayout.HorizontalScope())
			using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
			{
				Layer = EditorGUILayout.LayerField("Layer", Layer);

				if (changeCheckScope.changed)
				{
					Prefabs.Clear();
					Scenes.Clear();
				}

				NewLayer = EditorGUILayout.LayerField("New Layer", NewLayer);
			}
		}

		private void DrawPrefabs()
		{
			using (new EditorGUILayout.VerticalScope())
			{
				DrarHeader();
				DrawButtons();
				DrawList();
			}

			void DrarHeader() => EditorGUILayout.LabelField("Prefabs");

			void DrawButtons()
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					if (GUILayout.Button("Find"))
					{
						Find();
					}

					using (new EditorGUI.DisabledScope(Prefabs.Count == 0))
					{
						if (GUILayout.Button("Replace"))
						{
							Replace();
						}
					}
				}
			}

			void Find()
			{
				Prefabs.Clear();

				List<string> paths = PrefabTools.GetAllPrefabPaths().ToList();

				try
				{
					for (int i = 0; i < paths.Count; ++i)
					{
						string path = paths[i];

						float progress = i / (float)(paths.Count - 1);
						if (EditorUtility.DisplayCancelableProgressBar("Processing", path, progress))
						{
							break;
						}

						var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
						if (LayerUsedByPrefab(prefab))
						{
							Prefabs.Add(prefab);
						}
					}
				}
				finally
				{
					EditorUtility.ClearProgressBar();
				}

				bool LayerUsedByPrefab(GameObject prefab)
				{
					var transforms = prefab.GetComponentsInChildren<Transform>(true);

					foreach (Transform transform in transforms)
					{
						if (transform.gameObject.layer == Layer)
						{
							return true;
						}
					}

					return false;
				}
			}

			void Replace()
			{
				try
				{
					for (int i = 0; i < Prefabs.Count; ++i)
					{
						GameObject prefab = Prefabs[i];

						float progress = i / (float)(Prefabs.Count - 1);
						if (EditorUtility.DisplayCancelableProgressBar("Processing", prefab.name, progress))
						{
							break;
						}

						var transforms = prefab.GetComponentsInChildren<Transform>(true);

						foreach (Transform transform in transforms)
						{
							if (transform.gameObject.layer == Layer)
							{
								transform.gameObject.layer = NewLayer;
							}
						}

						AssetDatabase.SaveAssetIfDirty(prefab);
					}

					Prefabs.Clear();
				}
				finally
				{
					EditorUtility.ClearProgressBar();
				}
			}

			void DrawList()
			{
				using var scope = new EditorGUILayout.VerticalScope();

				if (Prefabs.Count > 0)
				{
					using var scrollViewScope = new EditorGUILayout.ScrollViewScope(PrefabsListScrollPosition);
					using var disabledScope = new EditorGUI.DisabledScope(true);
					PrefabsListScrollPosition = scrollViewScope.scrollPosition;
					foreach (GameObject prefab in Prefabs)
					{
						EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
					}
				}
				else
				{
					EditorGUILayout.HelpBox("None", MessageType.Info);
				}
			}
		}

		private void DrawScenes()
		{
			using (new EditorGUILayout.VerticalScope())
			{
				DrarHeader();
				DrawButtons();
				DrawList();
			}

			void DrarHeader() => EditorGUILayout.LabelField("Scenes");

			void DrawButtons()
			{
				if (GUILayout.Button("Find"))
				{
					Find();
				}
			}

			void Find()
			{
				var paths = AssetDatabase
					.FindAssets("t:scene", new[] { "Assets" })
					.Select(AssetDatabase.GUIDToAssetPath).ToList();

				Scenes.Clear();

				try
				{
					for (int i = 0; i < paths.Count; ++i)
					{
						string path = paths[i];

						float progress = i / (float)(paths.Count - 1);
						if (EditorUtility.DisplayCancelableProgressBar("Processing", path, progress))
						{
							break;
						}

						ScanScene(path);
					}

					void ScanScene(string path)
					{
						EditorSceneManager.OpenScene(path);

						var transforms = FindObjectsOfType<Transform>(true);

						foreach (Transform transform in transforms)
						{
							if (transform.gameObject.layer == Layer)
							{
								var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
								Scenes.Add(sceneAsset);
								return;
							}
						}
					}
				}
				finally
				{
					EditorUtility.ClearProgressBar();
				}
			}

			void DrawList()
			{
				using var scope = new EditorGUILayout.VerticalScope();

				if (Scenes.Count > 0)
				{
					using var scrollViewScope = new EditorGUILayout.ScrollViewScope(ScenesListScrollPosition);
					using var disabledScope = new EditorGUI.DisabledScope(true);
					ScenesListScrollPosition = scrollViewScope.scrollPosition;
					foreach (SceneAsset scene in Scenes)
					{
						EditorGUILayout.ObjectField(scene, typeof(SceneAsset), false);
					}
				}
				else
				{
					EditorGUILayout.HelpBox("None", MessageType.Info);
				}
			}
		}
	}
}
