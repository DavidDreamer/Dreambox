namespace Omniverse
{
	public class Item
	{
		public ItemDescription Description { get;  }
		
		private ItemPresenter Presenter { get; }

		public Item(ItemDescription description, ItemPresenter presenter)
		{
			Description = description;
			Presenter = presenter;
		}
	}
}
