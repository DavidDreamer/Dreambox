namespace Omniverse
{
	public abstract class Item<T> where T: ItemDesc
	{
		public T Desc { get;  }
		
		public ItemPresenter Presenter { get; }

		protected Item(T desc, ItemPresenter presenter)
		{
			Desc = desc;
			Presenter = presenter;
		}
	}
}
