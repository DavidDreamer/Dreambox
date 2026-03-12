using UnityEngine;
using Dreambox.Math;

namespace Dreambox.Rendering.Core
{
	public static class BlurUtils
	{
		public static ComputeBuffer CalculateGaussianWeights(int radius, float sigmaSqr)
		{
			const int maxDistance = 3;

			float[] weights = new float[radius * 2 + 1];
			ComputeBuffer computeBuffer = new(weights.Length, sizeof(float));

			float totalWeight = 0;

			for (int i = -radius; i <= radius; i++)
			{
				float distance = i / (float)radius * maxDistance;
				float weight = Gaussian.CalculateWeight(distance, sigmaSqr);
				weights[i + radius] = weight;
				totalWeight += weight;
			}

			for (int i = 0; i < weights.Length; i++)
			{
				weights[i] /= totalWeight;
			}

			computeBuffer.SetData(weights);

			return computeBuffer;
		}
	}
}
