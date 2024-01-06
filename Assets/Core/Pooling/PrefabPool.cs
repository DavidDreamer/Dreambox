using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Dreambox.Core.Pooling
{
	[Preserve]
	public class PrefabPool
	{
		private Dictionary<GameObject, Stack<GameObject>> Items { get; } = new();

		public GameObject Take(GameObject prefab)
		{
			if (!Items.ContainsKey(prefab))
			{
				Items.Add(prefab, new Stack<GameObject>());
			}

			if (Items[prefab].Count > 0)
			{
				GameObject instance = Items[prefab].Pop();
				instance.SetActive(true);
				return instance;
			}

			return Object.Instantiate(prefab);
		}

		public T Take<T>(T prefab) where T : MonoBehaviour, IPoolObject
		{
			GameObject gameObject = Take(prefab.gameObject);
			return gameObject.GetComponent<T>();
		}

		public void Return(GameObject prefab, GameObject instance)
		{
			instance.SetActive(false);
			Items[prefab].Push(instance);
		}

		public void Return<T>(T prefab, T instance) where T : MonoBehaviour, IPoolObject
		{
			instance.Cleanup();
			Return(prefab.gameObject, instance.gameObject);
		}
	}
}
