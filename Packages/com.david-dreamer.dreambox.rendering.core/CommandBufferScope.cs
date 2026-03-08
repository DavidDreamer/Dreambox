using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dreambox.Rendering.Core
{
	public readonly struct CommandBufferScope : IDisposable
	{
		public CommandBuffer CommandBuffer { get; }

		public CommandBufferScope(string name)
		{
			CommandBuffer = CommandBufferPool.Get(name);
		}

		public void Dispose()
		{
			Graphics.ExecuteCommandBuffer(CommandBuffer);
			CommandBufferPool.Release(CommandBuffer);
		}
	}
}
