using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class CurrencyDescription
	{
		[field: SerializeField]
		public string Name { get; private set; }
	}
}
