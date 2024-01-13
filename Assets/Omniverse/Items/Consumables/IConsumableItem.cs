namespace Omniverse
{
	public interface IConsumableItem
	{
		bool CanBeConsumed();

		void OnConsumed(Unit unit);
	}
}
