using UnityEngine;

namespace Omniverse
{
	[CreateAssetMenu]
	public class ItemDescription: ScriptableObject
	{
		[field: SerializeField]
		public ItemPresenter Prefab { get; private set; }
	}
}
