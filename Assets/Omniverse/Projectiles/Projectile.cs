using Dreambox.Math;
using UnityEngine;

namespace Omniverse
{
	[CreateAssetMenu]
	public class Projectile: ScriptableObject
	{
		[field: SerializeField]
		private ProjectilePresenter Prefab { get; set; }

		private ProjectilePresenter Presenter { get; set; }

		public void InstantiatePresenter(Vector3 position, Quaternion rotation)
		{
			Presenter = Instantiate(Prefab, position, rotation);
		}

		public void Launch(ParabolicTrajectory3D trajectory, Vector3 direction, float force) =>
			Presenter.Launch(trajectory, direction, force);
	}
}
