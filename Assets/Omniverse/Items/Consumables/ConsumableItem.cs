namespace Omniverse
{
	public interface IConsumableItem
	{
		bool CanBeConsumed();

		void OnConsumed(Unit unit);
	}
	
	public abstract class ConsumableItem<TDesc>: Item<TDesc>, IConsumableItem
		where TDesc: ConsumableItemDesc
	{
		protected ConsumableItem(TDesc desc): base(desc)
		{
		}
		
		public abstract bool CanBeConsumed();

		public abstract void OnConsumed(Unit unit);
	}
}
