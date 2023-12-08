using UnityEngine;

namespace Dreambox
{
	public class IntRangeLimitsAttribute: PropertyAttribute
	{
		public int Min { get; }

		public int Max { get; }

		public IntRangeLimitsAttribute(int min, int max)
		{
			Min = min;
			Max = max;
		}
	}
}
