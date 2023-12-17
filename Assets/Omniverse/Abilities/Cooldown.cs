using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Omniverse
{
	public class Cooldown
	{
		public CooldownDescriptor Descriptor { get; }
		
		public float TimeLeft { get; private set; }

		public float TimeLeftRatio { get; private set; }

		public bool IsActive { get; private set; }

		public Cooldown(CooldownDescriptor descriptor)
		{
			Descriptor = descriptor;
		}
		
		public async UniTask ActivateAsync(CancellationToken token)
		{
			IsActive = true;

			TimeLeft = Descriptor.Time;

			while (TimeLeft > 0f)
			{
				await UniTask.NextFrame(cancellationToken: token);
				TimeLeft = Mathf.Max(0, TimeLeft - Time.deltaTime);
				TimeLeftRatio = Mathf.Clamp01(TimeLeft / Descriptor.Time);
			}

			IsActive = false;
		}
	}
}
