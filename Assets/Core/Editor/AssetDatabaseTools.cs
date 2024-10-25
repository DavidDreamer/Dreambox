using UnityEngine;
using UnityEditor;

namespace Dreambox.Core.Editor
{
	public static class AssetDatabaseTools
	{
		[MenuItem("Dreambox/Assets/Remove Scriptable Objects With Missing Script")]
		public static void RemoveScriptableObjectsWithMissingScript()
		{
			Object[] assets = Selection.objects;
			foreach (Object asset in assets)
			{
				string assetPath = AssetDatabase.GetAssetPath(asset);
				AssetDatabase.RemoveScriptableObjectsWithMissingScript(assetPath);
			}
		}

		[MenuItem("Dreambox/Assets/Delete Selected SubAssets")]
		public static void DeleteSelectedSubAssets()
		{
			Object[] assets = Selection.objects;
			foreach (Object asset in assets)
			{
				if (!AssetDatabase.IsSubAsset(asset))
				{
					continue;
				}

				AssetDatabase.RemoveObjectFromAsset(asset);

				string assetPath = AssetDatabase.GetAssetPath(asset);
				AssetDatabase.ImportAsset(assetPath);
			}
		}
	}
}
