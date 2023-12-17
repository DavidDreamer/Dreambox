using UnityEngine;

namespace Omniverse
{
	public class Effect
	{
		public EffectDescriptor Descriptor { get; }

		public float Time { get; private set; }

		public bool OutOfTime => Time == 0;

		public Effect(EffectDescriptor descriptor)
		{
			Descriptor = descriptor;
			Time = descriptor.Time;
		}

		public void Tick(float deltaTime)
		{
			Time = Mathf.Max(0, Time - deltaTime);
		}
	}
}
