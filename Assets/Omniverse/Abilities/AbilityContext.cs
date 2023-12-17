using System.Collections.Generic;
using Dreambox.Math;
using UnityEngine;

namespace Omniverse
{
	public class AbilityContext
	{
		public Unit Caster { get; }

		public List<Unit> Units { get; } = new();

		public List<Vector3> Points { get; } = new();

		public List<ParabolicTrajectory3D> Trajectories { get; } = new();

		public AbilityContext(Unit caster)
		{
			Caster = caster;
		}

		public void Clear()
		{
			Units.Clear();
			Points.Clear();
			Trajectories.Clear();
		}
	}
}
