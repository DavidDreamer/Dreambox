using UnityEngine;
using UnityEngine.Rendering.Universal;
using Dreambox.Core;

namespace Dreambox.Rendering.URP
{
	public abstract class CustomRendererConfig : ScriptableObject
	{
		[field: SerializeField]
		public RenderPassEvent RenderPassEvent { get; private set; }

		[field: SerializeField]
		[field: EnumFlags]
		public CameraType CameraType { get; private set; }
	}
}
