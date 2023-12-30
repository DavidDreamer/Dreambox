using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dreambox.Math;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Omniverse.Abilities.Runtime
{
	[Serializable]
	public class LaunchProjectile: IAction
	{
		[field: SerializeField]
		private Projectile Projectile { get; set; }

		[field: SerializeField]
		private float Force { get; set; }

		public async UniTask Perform(AbilityContext context, CancellationToken token)
		{
			Projectile projectile = Object.Instantiate(Projectile);
			projectile.InstantiatePresenter(context.Caster.Position, Quaternion.identity);
			ParabolicTrajectory3D trajectory = context.Trajectories.First();
			projectile.Launch(trajectory, context.Caster.Presenter.transform.forward, Force);
			await UniTask.CompletedTask;
		}
	}
}
