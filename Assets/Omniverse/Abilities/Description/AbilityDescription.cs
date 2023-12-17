using System.Collections.Generic;
using Dreambox.Core;
using UnityEngine;

namespace Omniverse
{
	[CreateAssetMenu]
	public class AbilityDescription: ScriptableObject
	{
		[field: SerializeField]
		public Presentation Presentation { get; private set; }

		[field: SerializeField]
		public AbilityCastDescription Cast { get; private set; }
		
		[field: SerializeReference]
		public ITarget Target { get; private set; }

		[field: SerializeReference]
		public CooldownDescriptor Cooldown { get; private set; }

		[field: SerializeField]
		public List<AbilityCostDescription> Cost { get; private set; }
		
		[field: SerializeReference]
		[field: Versatile(typeof(IAction))]
		public List<IAction> Actions { get; private set; }
	}
}
