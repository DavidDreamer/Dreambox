using System;
using UnityEngine;

namespace Omniverse
{
	public static class ItemDescUtils
	{
		public static IItem Build(this ItemDesc itemDesc)
		{
			return itemDesc switch
			{
				CurrencyItemDesc desc => new CurrencyItem(desc),
				_ => throw new ArgumentOutOfRangeException(nameof(itemDesc))
			};
		}
	}
	
	public abstract class ItemDesc: ScriptableObject
	{
		[field: SerializeField]
		public ItemPresenter Prefab { get; private set; }
	}
}
