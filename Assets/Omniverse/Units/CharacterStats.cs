using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class CharacterStats
	{
		[field: SerializeField]
		public float MovementSpeed { get; private set; }

		[field: SerializeField]
		public float AngularSpeed { get; private set; }
	}
}
