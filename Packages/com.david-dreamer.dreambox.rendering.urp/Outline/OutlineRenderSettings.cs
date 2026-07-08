using Dreambox.Rendering.Core;
using Dreambox.Rendering.Universal;
using UnityEngine;

namespace Dreambox.Rendering
{
	[CreateAssetMenu(menuName = "Dreambox/Settings/Rendering/Outline")]
	public class OutlineRenderSettings : CustomRendererConfig
	{
		[field: SerializeField]
		public OutlineVariant[] Variants { get; private set; }
	}
}
