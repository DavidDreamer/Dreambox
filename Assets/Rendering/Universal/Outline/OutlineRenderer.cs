using System.Collections.Generic;

namespace Dreambox.Rendering.Universal
{
	public partial class OutlineRenderer : CustomRenderer<OutlineRendererConfig, OutlinePass>
	{
		public HashSet<OutlineTarget> Targets { get; } = new();

		protected override OutlinePass CreatePass() => new(this);

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
