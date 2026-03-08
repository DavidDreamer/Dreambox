using Dreambox.Core;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public abstract class CustomRendererConfig : ScriptableObject
	{
		[field: SerializeField]
		public RenderPassEvent RenderPassEvent { get; private set; }

		[field: SerializeField]
		[field: EnumFlags]
		public CameraType CameraType { get; private set; } = CameraType.SceneView | CameraType.Game;
	}
}
