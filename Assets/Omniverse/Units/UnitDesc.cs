using System.Collections.Generic;
using Omniverse.Abilities;
using UnityEngine;

namespace Omniverse
{
	[CreateAssetMenu(menuName = nameof(Omniverse) + "/" + nameof(UnitDesc), fileName = nameof(UnitDesc))]
	public class UnitDesc: ScriptableObject
	{
		[field: SerializeField]
		public Presentation Presentation { get; private set; }

		[field: SerializeField]
		public List<ResourceDesc> Resources { get; private set; }

		[field: SerializeField]
		public CharacterStats Stats { get; private set; }

		[field: SerializeField]
		public List<AbilityDesc> Abilities { get; private set; }
		
		[field: SerializeField]
		public List<LootDesc> Loot { get; private set; }
	}
}
