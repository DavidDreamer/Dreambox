using Dreambox.Core;
using UnityEngine;

namespace Omniverse
{
	[CreateAssetMenu]
	public class GlobalSettings: PreloadedScriptableObject<GlobalSettings>
	{
		[field: SerializeField]
		public Faction[] Factions { get; private set; }

		[field: SerializeField]
		public string[] Resources { get; private set; }
	}
}
