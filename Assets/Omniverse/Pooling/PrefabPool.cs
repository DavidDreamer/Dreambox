using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Omniverse
{
	[UnityEngine.Scripting.Preserve]
	public class PrefabPool
	{
		[Inject]
		private IObjectResolver ObjectResolver { get; set; }

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

			return CreateInstance(prefab);
		}

		public T Take<T>(T prefab) where T : MonoBehaviour, IPoolObject
		{
			GameObject gameObject = Take(prefab.gameObject);
			return gameObject.GetComponent<T>();
		}

		private GameObject CreateInstance(GameObject prefab)
		{
			GameObject instance = Object.Instantiate(prefab);
			ObjectResolver.InjectGameObject(instance);
			return instance;
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
