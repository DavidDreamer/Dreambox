﻿using UnityEngine;

namespace Dreambox.Core
{
	public class FloatRangeLimitsAttribute : PropertyAttribute
	{
		public float Min { get; }

		public float Max { get; }

		public FloatRangeLimitsAttribute(float min, float max)
		{
			Min = min;
			Max = max;
		}
	}
}
