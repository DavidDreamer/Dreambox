using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Omniverse
{
	[Preserve]
	public class Player
	{
		public Dictionary<int, AsyncReactiveProperty<int>> Currencies { get; } = new();

		public Unit Unit { get; set; }

		public void ChangeCurrency(int id, int amount)
		{
			if (Currencies.ContainsKey(id) is false)
			{
				Currencies.Add(id, new AsyncReactiveProperty<int>(0));
			}

			Currencies[id].Value += amount;
		}
	}
}
