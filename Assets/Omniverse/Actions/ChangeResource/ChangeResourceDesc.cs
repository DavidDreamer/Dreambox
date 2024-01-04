using System;
using UnityEngine;

namespace Omniverse.Actions
{
	[Serializable]
	public class ChangeResourceDesc: IActionDesc
	{
		[field: SerializeField]
		[field: Resource]
		public int ResourceID { get; private set; }

		[field: SerializeField]
		public int Amount { get; private set; }
	}
}
