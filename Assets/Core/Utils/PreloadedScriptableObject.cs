using System;
using System.Linq;
using UnityEngine;

namespace Dreambox.Core
{
	public class PreloadedScriptableObject<T>: ScriptableObject where T : PreloadedScriptableObject<T>
	{
		public static T Instance { get; private set; }

#if UNITY_EDITOR
		protected virtual void OnValidate()
		{
			var preloadedAssets = UnityEditor.PlayerSettings.GetPreloadedAssets();
			if (preloadedAssets.Contains(this))
			{
				return;
			}

			var tempList = preloadedAssets.ToList();
			tempList.Add(this);
			preloadedAssets = tempList.ToArray();

			UnityEditor.PlayerSettings.SetPreloadedAssets(preloadedAssets);
		}
#endif

		private void OnEnable()
		{
			if (Instance != null && Instance != this)
			{
				throw new Exception($"Multiple instances of {typeof(T).Name} detected");
			}

			Instance = (T)this;
		}
	}
}
