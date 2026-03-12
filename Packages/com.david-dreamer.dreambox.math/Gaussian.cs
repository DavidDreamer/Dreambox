using UnityEngine;

namespace Dreambox.Math
{
	public static class Gaussian
	{
		public static float CalculateWeight(float distance, float sigmaSqr)
		{
			float a = 1.0f / Mathf.Sqrt(2 * Mathf.PI * sigmaSqr);
			float b = Mathf.Exp(-(distance * distance) / (2 * sigmaSqr));
			return a * b;
		}
	}
}
