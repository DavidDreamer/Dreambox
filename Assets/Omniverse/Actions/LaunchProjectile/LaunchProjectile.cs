using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dreambox.Math;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class LaunchProjectile: Action<LaunchProjectileDesc>
	{
		public LaunchProjectile(LaunchProjectileDesc desc): base(desc)
		{
		}

		public override async UniTask Perform(ExecutionContext context, CancellationToken token)
		{
			Projectile projectile = Object.Instantiate(Desc.Projectile);
			projectile.InstantiatePresenter(context.Caster.Position, Quaternion.identity);
			ParabolicTrajectory3D trajectory = context.Trajectories.First();
			projectile.Launch(trajectory, context.Caster.Presenter.transform.forward, Desc.Force);
			await UniTask.CompletedTask;
		}
	}
}
