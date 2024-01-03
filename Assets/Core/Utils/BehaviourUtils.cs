using UnityEngine;

namespace Dreambox.Core
{
	public static class BehaviourUtils
	{
		public static BehaviourActivationScope GetActivationScope(this Behaviour behaviour, bool enabled) =>
			new(behaviour, enabled);
	}
}
