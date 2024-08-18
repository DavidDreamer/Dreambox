using System;
using UnityEngine;

namespace Dreambox.Core
{
	public readonly struct MeshRendererActivationScope : IDisposable
	{
		private MeshRenderer MeshRenderer { get; }

		private bool PreviousState { get; }

		public MeshRendererActivationScope(MeshRenderer meshRenderer, bool enabled)
		{
			MeshRenderer = meshRenderer;

			PreviousState = MeshRenderer.enabled;
			MeshRenderer.enabled = enabled;
		}

		public void Dispose()
		{
			MeshRenderer.enabled = PreviousState;
		}
	}
}
