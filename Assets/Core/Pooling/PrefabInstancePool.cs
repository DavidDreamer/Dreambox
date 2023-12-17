using System.Collections.Generic;
using UnityEngine;

namespace Dreambox.Core.Pooling
{
	public class PrefabInstancePool
	{
		private Dictionary<GameObject, Stack<GameObject>> Items { get; } = new();

		public GameObject Get(GameObject prefab)
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

		public T Get<T>(T prefab) where T : MonoBehaviour, IPoolable
		{
			GameObject gameObject = Get(prefab.gameObject);
			return gameObject.GetComponent<T>();
		}

		public void Release(GameObject prefab, GameObject instance)
		{
			instance.SetActive(false);
			Items[prefab].Push(instance);
		}

		public void Release<T>(T prefab, T instance) where T : MonoBehaviour, IPoolable
		{
			instance.Cleanup();
			Release(prefab.gameObject, instance.gameObject);
		}
	}
}
