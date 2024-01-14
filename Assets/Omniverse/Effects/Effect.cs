using UnityEngine;

namespace Omniverse
{
	public class Effect
	{
		public EffectDesc Desc { get; }

		public float Time { get; private set; }

		public bool OutOfTime => Time == 0;

		public Effect(EffectDesc desc)
		{
			Desc = desc;
			Time = desc.Time;
		}

		public void Tick(float deltaTime)
		{
			Time = Mathf.Max(0, Time - deltaTime);
		}
	}
}
