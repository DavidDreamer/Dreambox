using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Dreambox.Rendering.Core
{
	public class ConstantComputeBuffer<T> : IDisposable where T : struct
	{
		private ComputeBuffer ComputeBuffer { get; }

		private T[] Data { get; }

		public ConstantComputeBuffer(int id)
		{
			ComputeBuffer = new ComputeBuffer(1, UnsafeUtility.SizeOf<T>(), ComputeBufferType.Constant);
			Data = new T[1];
			Shader.SetGlobalConstantBuffer(id, ComputeBuffer, 0, ComputeBuffer.stride);
		}

		public void SetData(T data)
		{
			Data[0] = data;
			ComputeBuffer.SetData(Data);
		}

		public void Dispose()
		{
			ComputeBuffer.Release();
		}
	}
}
