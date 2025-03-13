using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dreambox.Rendering.Universal
{
	public partial class OutlineRenderer : CustomRenderer<OutlineRendererConfig, OutlinePass>
	{
		public HashSet<OutlineTarget> Targets { get; } = new();

		private Material Material { get; set; }

		private ComputeBuffer VariantsBuffer { get; set; }

		protected override OutlinePass Setup()
		{
			Material = CoreUtils.CreateEngineMaterial(Config.Shader);
			VariantsBuffer = new ComputeBuffer(Config.Variants.Length, Marshal.SizeOf<OutlineVariant>());
			Material.SetBuffer(OutlineShaderVariables.VariantsBuffer, VariantsBuffer);

			float width = Config.Variants.Max(config => config.Width);
			VariantsBuffer.SetData(Config.Variants);

			OutlinePass pass = new(Material, Targets, width);
			return pass;
		}

		protected override void Cleanup()
		{
			CoreUtils.Destroy(Material);
			VariantsBuffer.Release();
		}

		public void AddTarget(OutlineTarget target)
		{
			Targets.Add(target);
		}

		public void RemoveTarget(OutlineTarget target)
		{
			Targets.Remove(target);
		}

		public void Clear() => Targets.Clear();

		protected override bool IsInactive() => Targets.Count == 0;
	}
}
