using UnityEngine;

namespace Omniverse
{
	[CreateAssetMenu]
	public class EffectDescriptor: ScriptableObject
	{
		[field: SerializeField]
		public Sprite Icon { get; private set; }
		
		[field: SerializeField]
		public bool IsPositive { get; private set; }

		[field: SerializeField]
		public float Time { get; private set; }
		
		[field: SerializeField]
		public UnitStatus UnitStatus { get; private set; }
	}
}
