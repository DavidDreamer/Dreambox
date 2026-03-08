using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Dreambox.Core.Editor
{
	public static class PrefabTools
	{
		public static IEnumerable<string> GetAllPrefabPaths() => AssetDatabase
			.FindAssets("t:prefabs", new[] { "Assets" }).Select(AssetDatabase.GUIDToAssetPath);
	}
}
