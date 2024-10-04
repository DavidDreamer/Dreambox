using System;
using UnityEngine;

namespace Dreambox.Core
{
	//todo: move to separate file
	public enum ArithmeticOperation
	{
		Addition,
		Subtraction,
		Multiplication,
		Division
	}

	public static class MatrixUtils
	{
		public static Matrix4x4 WorldUpRotation = Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0));
	}

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
