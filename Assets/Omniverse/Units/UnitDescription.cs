using System.Collections.Generic;
using Omniverse.Abilities.Description;
using UnityEngine;

namespace Omniverse
{
	[CreateAssetMenu(menuName = nameof(Omniverse) + "/" + nameof(UnitDescription), fileName = nameof(UnitDescription))]
	public class UnitDescription: ScriptableObject
	{
		[field: SerializeField]
		public Presentation Presentation { get; private set; }

		[field: SerializeField]
		public List<ResourceDescriptor> Resources { get; private set; }

		[field: SerializeField]
		public CharacterStats Stats { get; private set; }

		[field: SerializeField]
		public List<Ability> AbilityDescriptions { get; private set; }
		
		[field: SerializeField]
		public List<LootDescription> Loot { get; private set; }
	}
}
