namespace Omniverse
{
	public interface IItem
	{
		ItemPresenter Presenter { get; set; }
	}
	
	public abstract class Item<TDesc>: IItem
		where TDesc: ItemDesc
	{
		public TDesc Desc { get;  }
		
		public ItemPresenter Presenter { get; set; }

		protected Item(TDesc desc)
		{
			Desc = desc;
		}
	}
}
