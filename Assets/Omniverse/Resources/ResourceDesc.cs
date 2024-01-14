using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class ResourceDesc
	{
		[field: SerializeField]
		[field: Resource]
		public int ID { get; private set; }

		[field: SerializeField]
		public float Capacity { get; private set; }

		[field: SerializeField]
		public float Regeneration { get; private set; }

		[field: SerializeField]
		public bool Vital { get; private set; }
	}
}
