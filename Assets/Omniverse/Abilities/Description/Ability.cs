using System.Collections.Generic;
using Dreambox.Core;
using Omniverse.Actions;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[CreateAssetMenu(menuName = nameof(Omniverse) + "/" + nameof(ItemDescription), fileName = nameof(ItemDescription))]
	public class Ability: ScriptableObject
	{
		[field: SerializeField]
		public Presentation Presentation { get; private set; }

		[field: SerializeField]
		public Cast Cast { get; private set; }

		[field: SerializeReference]
		[field: Versatile(typeof(ITarget))]
		public ITarget Target { get; private set; }

		[field: SerializeField]
		public Cooldown Cooldown { get; private set; }

		[field: SerializeField]
		public List<Cost> Cost { get; private set; }

		[field: SerializeReference]
		[field: Versatile(typeof(IActionDesc))]
		public IActionDesc[] Actions { get; private set; }
	}
}
