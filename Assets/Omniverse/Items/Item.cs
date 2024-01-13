namespace Omniverse
{
	public interface IItem
	{
		ItemPresenter Presenter { get; }
	}
	
	public abstract class Item<TDesc>: IItem
		where TDesc: ItemDesc
	{
		public TDesc Desc { get;  }
		
		public ItemPresenter Presenter { get; }

		protected Item(TDesc desc, ItemPresenter presenter)
		{
			Desc = desc;
			Presenter = presenter;
		}
	}
}
