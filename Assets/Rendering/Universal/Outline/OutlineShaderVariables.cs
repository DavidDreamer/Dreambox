// Copyright (c) Saber BGS 2022. All rights reserved.
// ---------------------------------------------------------------------------------------------

using UnityEngine;

namespace Dreambox.Rendering.Universal
{
	public static class OutlineShaderVariables
	{
		public static int StepWidth { get; } = Shader.PropertyToID(nameof(StepWidth));

		public static int VariantsBuffer { get; } = Shader.PropertyToID(nameof(VariantsBuffer));

		public static int Variant { get; } = Shader.PropertyToID(nameof(Variant));

		public static int BaseMap { get; } = Shader.PropertyToID($"_{nameof(BaseMap)}");
	}
}
