using UnityEngine;

namespace Dreambox.Core
{
	public static class ActivationScopeUtils
	{
		public static BehaviourActivationScope GetActivationScope(this Behaviour behaviour, bool enabled) =>
			new(behaviour, enabled);

		public static MeshRendererActivationScope GetActivationScope(this MeshRenderer meshRenderer, bool enabled) =>
			new(meshRenderer, enabled);
	}
}
