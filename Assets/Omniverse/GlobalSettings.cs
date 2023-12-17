using UnityEngine;

namespace Omniverse
{
	public class GlobalSettings: ScriptableObject
	{
		[field: SerializeField]
		public string[] Resources { get; private set; }
	}
}
