using Dreambox.Core;
using UnityEngine;

namespace Omniverse
{
	[CreateAssetMenu]
	public class GlobalSettings: PreloadedScriptableObject<GlobalSettings>
	{
		[field: SerializeField]
		public CurrencyDesc[] Currencies { get; private set; }
		
		[field: SerializeField]
		public FactionDesc[] Factions { get; private set; }

		[field: SerializeField]
		public string[] Resources { get; private set; }
		
		[field: Header("Physics")]
		[field: Layer]
		[field: SerializeField]
		public int HitboxLayer { get; private set; }
		
		[field: SerializeField]
		[field: HideInInspector]
		public LayerMask HitboxLayerMask { get; private set; }
		
#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			HitboxLayerMask = LayerMaskUtils.NumberToMask(HitboxLayer);
		}
#endif
	}
}
