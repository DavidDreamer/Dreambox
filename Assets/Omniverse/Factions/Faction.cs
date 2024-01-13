using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Omniverse
{
	public class Faction
	{
		public int ID { get; }
		
		private FactionDesc Desc { get; }
		
		public Dictionary<int, AsyncReactiveProperty<int>> Currencies { get; } = new();

		public Faction(int id, FactionDesc desc)
		{
			ID = id;
			Desc = desc;
		}

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
