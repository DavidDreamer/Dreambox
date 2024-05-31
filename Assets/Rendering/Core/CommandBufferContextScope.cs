using System;
using UnityEngine.Rendering;

namespace Dreambox.Rendering.Core
{
	public readonly struct CommandBufferContextScope: IDisposable
	{
		public  ScriptableRenderContext Context { get; }
		
		public CommandBuffer CommandBuffer { get; }

		public CommandBufferContextScope(ScriptableRenderContext context, string name)
		{
			Context = context;
			CommandBuffer = CommandBufferPool.Get(name);
		}

		public void Dispose()
		{
			Context.ExecuteCommandBuffer(CommandBuffer);
			CommandBufferPool.Release(CommandBuffer);
		}
	}
}
