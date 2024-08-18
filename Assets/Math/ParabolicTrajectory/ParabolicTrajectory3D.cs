using UnityEngine;

namespace Dreambox.Math
{
	public class ParabolicTrajectory3D
	{
		public Vector3 Start { get; set; }

		public Vector3 End { get; set; }

		public ParabolicTrajectoryParameters Parameters { get; set; }

		public Vector3 EvaluatePosition(float time)
		{
			Vector2 position =
				ParabolicTrajectory.EvaluatePosition(Parameters.Angle, Parameters.Velocity, Parameters.Time * time);

			Vector3 direction = End - Start;
			Vector3 deltaHotizontal = new Vector3(direction.x, 0, direction.z).normalized * position.x;

			var delta = new Vector3(deltaHotizontal.x, position.y, deltaHotizontal.z);

			return Start + delta;
		}

		public static bool Create(Vector3 start, Vector3 end, float height, out ParabolicTrajectory3D trajectory)
		{
			Vector3 direction = end - start;
			var directionHorizontal = new Vector3(direction.x, 0, direction.z);
			var target = new Vector2(directionHorizontal.magnitude, direction.y);

			if (ParabolicTrajectory.EvaluateTrajectoryParameters(target, height, out ParabolicTrajectoryParameters parameters))
			{
				trajectory = new ParabolicTrajectory3D
				{
					Start = start,
					End = end,
					Parameters = parameters
				};

				return true;
			}

			trajectory = default;
			return false;
		}

		public static float EvaluateTime(Vector3 start, Vector3 end, float angle, float velocity)
		{
			Vector3 direction = end - start;
			float distance = new Vector3(direction.x, 0, direction.z).magnitude;
			return ParabolicTrajectory.EvaluateTime(distance, angle, velocity);
		}
	}
}
