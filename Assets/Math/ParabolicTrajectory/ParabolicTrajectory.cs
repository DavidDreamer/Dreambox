using UnityEngine;

namespace Dreambox.Math
{
	public static class ParabolicTrajectory
	{
		private const float Gravity = 9.81f;

		private const float GravityDouble = Gravity * 2f;
		
		private const float GravityHalf = Gravity * 0.5f;
		
		public static Vector2 EvaluatePosition(float angle, float velocity, float time)
		{
			float velocityTime = velocity * time;
			float x = Mathf.Cos(angle) * velocityTime;
			float y = Mathf.Sin(angle) * velocityTime - GravityHalf * time * time;
			return new Vector2(x, y);
		}

		public static float EvaluateAngle(float velocity, float height) =>
			Mathf.Asin(Mathf.Sqrt(GravityDouble * height) / velocity);

		public static float EvaluateTime(float distance, float angle, float velocity) =>
			distance / (velocity * Mathf.Cos(angle));

		public static bool EvaluateTrajectoryParameters(
			Vector2 target,
			float height,
			out ParabolicTrajectoryParameters parameters)
		{
			float a = -GravityHalf;
			float b = Mathf.Sqrt(GravityDouble * height);
			float c = -target.y;

			float time = QuadraticEquation.Calculate(a, b, c, true);

			if (float.IsNaN(time))
			{
				parameters = default;
				return false;
			}
			
			float angle = Mathf.Atan(b * time / target.x);
			float velocity = b / Mathf.Sin(angle);

			parameters = new ParabolicTrajectoryParameters
			{
				Time = time,
				Angle = angle,
				Velocity = velocity
			};

			return true;
		}
	}
}
