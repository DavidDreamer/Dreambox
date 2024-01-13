namespace Omniverse
{
	public interface IConsumableItem
	{
		bool CanBeConsumed();

		void OnConsumed(Unit unit);
	}
	
	public abstract class ConsumableItem<TDesc>: Item<TDesc>
		where TDesc: ConsumableItemDesc
	{
		protected ConsumableItem(TDesc desc, ItemPresenter presenter): base(desc, presenter)
		{
		}
		
		public abstract bool CanBeConsumed();

		public abstract void OnConsumed(Unit unit);
	}
}
