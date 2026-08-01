using UnityEngine;

namespace Dreambox.Core
{
	public class RangeOddAttribute : PropertyAttribute
	{
		public readonly int min;
		public readonly int max;

		public RangeOddAttribute(int min, int max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
