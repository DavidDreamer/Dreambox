using UnityEngine;

namespace Omniverse
{
	public abstract class ItemPresenter: MonoBehaviour
	{
	}
	
	public abstract class ItemPresenter<TItem>: ItemPresenter 
	{
		public TItem Item { get; set; }
	}
}
