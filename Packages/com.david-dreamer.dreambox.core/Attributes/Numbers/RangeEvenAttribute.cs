using UnityEngine;

namespace Dreambox.Core
{
    public class RangeEvenAttribute : PropertyAttribute
	{
		public readonly int min;
		public readonly int max;

		public RangeEvenAttribute(int min, int max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
