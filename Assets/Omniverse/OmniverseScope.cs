using System.Linq;
using VContainer;
using VContainer.Unity;

namespace Omniverse
{
	public class OmniverseScope: LifetimeScope
	{
		protected override void Configure(IContainerBuilder builder)
		{
			builder.Register<PrefabPool>(Lifetime.Singleton);
			builder.RegisterEntryPoint<FactionManager>().AsSelf().WithParameter(GlobalSettings.Instance.Factions.ToList());
			builder.RegisterEntryPoint<ItemManager>().AsSelf();
			builder.RegisterEntryPoint<UnitManager>().AsSelf();
		}
	}
}
