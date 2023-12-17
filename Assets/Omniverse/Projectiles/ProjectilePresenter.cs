using System.Threading;
using Cysharp.Threading.Tasks;
using Dreambox.Math;
using UnityEngine;

namespace Omniverse
{
	public class ProjectilePresenter: MonoBehaviour
	{
		public void Launch(ParabolicTrajectory3D trajectory, Vector3 direction, float force)
		{
			transform.forward = direction;

			LaunchAsync(destroyCancellationToken).SuppressCancellationThrow();

			async UniTask LaunchAsync(CancellationToken token)
			{
				float time = 0;
				
				while (true)
				{
					await UniTask.WaitForFixedUpdate(token);

					time += force * Time.fixedDeltaTime;
					
					transform.position = trajectory.EvaluatePosition(time / trajectory.Parameters.Time);
				}
			}
		}
	}
}
