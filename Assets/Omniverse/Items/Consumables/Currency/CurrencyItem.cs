using VContainer;
using Object = UnityEngine.Object;

namespace Omniverse
{
	public class CurrencyItem: ConsumableItem<CurrencyItemDesc>
	{
		[Inject]
		private FactionManager FactionManager { get; set; }

		public CurrencyItem(CurrencyItemDesc desc): base(desc)
		{
		}
		
		public override bool CanBeConsumed() => true;

		public override void OnConsumed(Unit unit)
		{
			Faction faction = FactionManager.Factions[unit.FactionID];
			faction.ChangeCurrency(Desc.CurrencyID, Desc.Amount);

			Object.Destroy(Presenter);
		}
	}
}
