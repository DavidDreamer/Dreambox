using System;
using UnityEngine;

namespace Dreambox.Core
{
	[Serializable]
	public struct IntRange
	{
		[field: SerializeField]
		public int Min { get; set; }

		[field: SerializeField]
		public int Max { get; set; }

		public int Random() => UnityEngine.Random.Range(Min, Max + 1);

		public int Evaluate(float value) => Mathf.RoundToInt(Mathf.Lerp(Min, Max, value));
	}
}
