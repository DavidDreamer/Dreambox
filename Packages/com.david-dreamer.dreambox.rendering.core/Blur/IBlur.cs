using System;
using UnityEngine.Rendering;

namespace Dreambox.Rendering.Core
{
	public interface IBlur : IDisposable
	{
		RTHandle Result { get; }

		void Execute(CommandBuffer commandBuffer, RTHandle source);
	}
}
