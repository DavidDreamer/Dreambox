// Copyright (c) Saber BGS 2023. All rights reserved.
// ---------------------------------------------------------------------------------------------

using Dreambox.Rendering.Universal;
using UnityEngine;

namespace Dreambox.Rendering
{
	[CreateAssetMenu(menuName = "Dreambox/Settings/Rendering/Outline")]
	public class OutlineRenderSettings : CustomRendererConfig
	{
		[field: SerializeField]
		public Material Material { get; private set; }

		[field: SerializeField]
		public OutlineVariant[] Variants { get; private set; }
	}
}
