using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class FactionDesc
	{
		[field: SerializeField]
		public string Name { get; private set; }
	}
}
