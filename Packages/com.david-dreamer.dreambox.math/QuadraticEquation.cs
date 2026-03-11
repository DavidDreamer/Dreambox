using UnityEngine;

namespace Dreambox.Math
{
	public static class QuadraticEquation
	{
		private static float CalculateDescriminant(float a, float b, float c) => b * b - 4 * a * c;

		private static float CalculateRoot(float a, float b) => -(b / 2 * a);

		private static float CalculateRoot(float a, float b, float d, float sign) =>
			(-b + sign * Mathf.Sqrt(d)) / (2 * a);

		public static float Calculate(float a, float b, float c, bool max)
		{
			float d = CalculateDescriminant(a, b, c);

			switch (d)
			{
				case float.NaN:
					return float.NaN;
				case 0:
					return CalculateRoot(a, b);
				default:
					float first = CalculateRoot(a, b, d, 1);
					float second = CalculateRoot(a, b, d, -1);
					return max ? Mathf.Max(first, second) : Mathf.Min(first, second);
			}
		}
	}
}
