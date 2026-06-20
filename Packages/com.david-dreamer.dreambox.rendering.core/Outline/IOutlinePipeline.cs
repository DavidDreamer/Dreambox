using System.Collections.Generic;
using UnityEngine.Rendering;
using System;

namespace Dreambox.Rendering.Core
{
	public interface IOutlinePipeline : IDisposable
	{
		void Mask(CommandBuffer commandBuffer, HashSet<OutlineRenderer> renderers);
		void Initialize(CommandBuffer commandBuffer);
		void JumpFlood(CommandBuffer commandBuffer);
		void Decode(CommandBuffer commandBuffer, RTHandle target);
	}
}
