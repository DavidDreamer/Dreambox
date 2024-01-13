namespace Omniverse
{
	public abstract class Item<TDesc> 
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
