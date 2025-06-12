using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Dreambox.Rendering.Universal
{
	public partial class OutlineRenderer : CustomRenderer<OutlineRenderSettings, OutlinePass>
	{
		public HashSet<OutlineTarget> Targets { get; } = new();

		private ComputeBuffer VariantsBuffer { get; set; }

		protected override OutlinePass Setup()
		{
			VariantsBuffer = new ComputeBuffer(Config.Variants.Length, Marshal.SizeOf<OutlineVariant>());
			Config.Material.SetBuffer(OutlineShaderVariables.VariantsBuffer, VariantsBuffer);

			float width = Config.Variants.Max(config => config.Width);
			VariantsBuffer.SetData(Config.Variants);

			OutlinePass pass = new(Config.Material, Targets, width);
			return pass;
		}

		protected override void Cleanup()
		{
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
