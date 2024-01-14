using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Omniverse.Abilities
{
	public class Cooldown
	{
		public CooldownDesc Desc { get; }
		
		public float TimeLeft { get; private set; }

		public float TimeLeftRatio { get; private set; }

		public bool IsActive { get; private set; }

		public Cooldown(CooldownDesc desc)
		{
			Desc = desc;
		}
		
		public async UniTask ActivateAsync(CancellationToken token)
		{
			IsActive = true;

			TimeLeft = Desc.Time;

			while (TimeLeft > 0f)
			{
				await UniTask.NextFrame(cancellationToken: token);
				TimeLeft = Mathf.Max(0, TimeLeft - Time.deltaTime);
				TimeLeftRatio = Mathf.Clamp01(TimeLeft / Desc.Time);
			}

			IsActive = false;
		}
	}
}
