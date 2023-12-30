using System.Collections.Generic;
using Dreambox.Core;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[CreateAssetMenu]
	public class Ability: ScriptableObject
	{
		[field: SerializeField]
		public Presentation Presentation { get; private set; }

		[field: SerializeField]
		public Cast Cast { get; private set; }

		[field: SerializeReference]
		[field: Versatile(typeof(ITarget))]
		public ITarget Target { get; private set; }

		[field: SerializeReference]
		public Cooldown Cooldown { get; private set; }

		[field: SerializeField]
		public List<Cost> Cost { get; private set; }

		[field: SerializeReference]
		[field: Versatile(typeof(IAction))]
		public List<IAction> Actions { get; private set; }
	}
}
