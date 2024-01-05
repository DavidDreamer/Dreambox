using System;
using UnityEngine;

namespace Dreambox.Core
{
	public static class LayerMaskUtils
	{
		public static LayerMask NumberToMask(int number)
		{
			if (number is < 0 or > 31)
			{
				throw new ArgumentOutOfRangeException(number.ToString());
			}
			
			string layerName = LayerMask.LayerToName(number);
			int bitMask = LayerMask.GetMask(layerName);
			return bitMask;
		}
	}
}
