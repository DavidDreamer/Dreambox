using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class CurrencyDesc
	{
		[field: SerializeField]
		public string Name { get; private set; }
	}
}
