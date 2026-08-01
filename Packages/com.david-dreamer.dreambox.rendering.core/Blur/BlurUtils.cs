using UnityEngine;
using Dreambox.Math;

namespace Dreambox.Rendering.Core
{
	public static class BlurUtils
	{
		public static ComputeBuffer CalculateGaussianKernel(int kernelSize, float sigma)
		{
			float[] weights = new float[kernelSize];
			ComputeBuffer computeBuffer = new(weights.Length, sizeof(float));

			int radius = kernelSize / 2;
			float sigmaSqr = sigma * sigma;

			float totalWeight = 0;

			for (int i = 0; i < kernelSize; i++)
			{
				float distance = i - radius;
				float weight = Gaussian.CalculateWeight(distance, sigmaSqr);
				weights[i] = weight;
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
