using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dreambox.Core.Editor
{
	public static class TerrainUtils
	{
		[MenuItem("Tools/Dreambox/Terrain/Export Trees")]
		public static void ExportTrees()
		{
			IEnumerable<Terrain> terrains = Selection.gameObjects.
				Select(gameObject => gameObject.GetComponent<Terrain>()).
				Where(terrain => terrain != null);

			foreach (Terrain terrain in terrains)
			{
				TerrainData terrainData = terrain.terrainData;

				Transform treesHolder = GetOrCreateTreesHolder(terrain);

				foreach (TreeInstance treeInstance in terrain.terrainData.treeInstances)
				{
					TreePrototype treePrototype = terrain.terrainData.treePrototypes[treeInstance.prototypeIndex];

					Vector3 position = new Vector3(treeInstance.position.x * terrainData.size.x,
										   treeInstance.position.y * terrainData.size.y,
										   treeInstance.position.z * terrainData.size.z) +
									   terrain.transform.position;

					Quaternion rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * treeInstance.rotation, Vector3.up);

					Vector3 prefabScale = treePrototype.prefab.transform.lossyScale;
					Vector3 treeScale = new(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale);
					Vector3 scale = Vector3.Scale(prefabScale, treeScale);

					var prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(treePrototype.prefab, treesHolder);
					prefabInstance.transform.position = position;
					prefabInstance.transform.rotation = rotation;
					prefabInstance.transform.localScale = scale;
				}

				terrain.terrainData.treeInstances = Array.Empty<TreeInstance>();

				EditorUtility.SetDirty(terrain);
			}
		}

		[MenuItem("Tools/Dreambox/Terrain/Import Trees")]
		public static void ImportTrees()
		{
			IEnumerable<Terrain> terrains = Selection.gameObjects.
			   Select(gameObject => gameObject.GetComponent<Terrain>()).
			   Where(terrain => terrain != null);

			foreach (Terrain terrain in terrains)
			{
				TerrainData terrainData = terrain.terrainData;

				Transform treesHolder = GetOrCreateTreesHolder(terrain);
				var treeInstances = new TreeInstance[treesHolder.childCount];

				for (int i = 0; i < treesHolder.childCount; ++i)
				{
					Transform tree = treesHolder.GetChild(i);
					GameObject treePrefab = PrefabUtility.GetCorrespondingObjectFromSource(tree).gameObject;
					int treePrototypeIndex = terrainData.GetTreePrototypeIndex(treePrefab);
					Vector3 position = tree.position - terrain.transform.position;
					position = new Vector3(
						position.x / terrainData.size.x,
						position.y / terrainData.size.y,
						position.z / terrainData.size.z);
					float rotation = tree.rotation.y * Mathf.Deg2Rad;
					float widthScale = tree.localScale.x / treePrefab.transform.lossyScale.x;
					float heightScale = tree.localScale.y / treePrefab.transform.lossyScale.y;

					var treeInstance = new TreeInstance
					{
						position = position,
						rotation = rotation,
						widthScale = widthScale,
						heightScale = heightScale,
						prototypeIndex = treePrototypeIndex
					};

					treeInstances[i] = treeInstance;
				}

				terrainData.treeInstances = treeInstances;

				Object.DestroyImmediate(treesHolder.gameObject);

				EditorUtility.SetDirty(terrain);
			}
		}

		private static int GetTreePrototypeIndex(this TerrainData terrainData, GameObject prefab)
		{
			for (var i = 0; i < terrainData.treePrototypes.Length; ++i)
			{
				TreePrototype treePrototype = terrainData.treePrototypes[i];
				if (treePrototype.prefab == prefab)
				{
					return i;
				}
			}

			return -1;
		}

		private static Transform GetOrCreateTreesHolder(Terrain terrain)
		{
			const string name = "Trees";

			Transform holder = terrain.transform.Find(name);
			if (holder == null)
			{
				holder = new GameObject(name).transform;
				holder.SetParent(terrain.transform, false);
			}

			return holder;
		}
	}
}
