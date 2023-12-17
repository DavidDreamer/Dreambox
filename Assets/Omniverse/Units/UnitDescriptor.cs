using System.Collections.Generic;
using UnityEngine;

namespace Omniverse
{
	[CreateAssetMenu]
	public class UnitDescriptor: ScriptableObject
	{
		[field: SerializeField]
		public Presentation Presentation { get; private set; }

		[field: SerializeField]
		public List<ResourceDescriptor> Resources { get; private set; }

		[field: SerializeField]
		public CharacterStats Stats { get; private set; }

		[field: SerializeField]
		public List<AbilityDescription> AbilityDescriptions { get; private set; }
	}
}
