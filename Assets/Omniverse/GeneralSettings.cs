using UnityEngine;

namespace Omniverse
{
	public class GeneralSettings: ScriptableObject
	{
		[field: SerializeField]
		public Faction[] Factions { get; private set; }
		
		[field: SerializeField]
		public string[] Resources { get; private set; }
	}
}
