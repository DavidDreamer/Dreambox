using System;
using UnityEngine;

namespace Dreambox.Core
{
	public readonly struct BehaviourActivationScope : IDisposable
	{
		private Behaviour Behaviour { get; }

		private bool PreviousState { get; }

		public BehaviourActivationScope(Behaviour behaviour, bool enabled)
		{
			Behaviour = behaviour;

			PreviousState = Behaviour.enabled;
			Behaviour.enabled = enabled;
		}

		public void Dispose()
		{
			Behaviour.enabled = PreviousState;
		}
	}
}
