﻿// Copyright (c) Saber BGS 2023. All rights reserved.
// ---------------------------------------------------------------------------------------------

using Dreambox.Rendering.URP;
using UnityEngine;

namespace Dreambox.Rendering
{
	[CreateAssetMenu(menuName = "Dreambox/Rendering/OutlineConfig", fileName = nameof(OutlineConfig))]
	public class OutlineConfig: ScriptableObject
	{
		[field: SerializeField]
		public Shader Shader { get; private set; }
		
		[field: SerializeField]
		public OutlineVariant[] Variants { get; private set; }
	}
}
