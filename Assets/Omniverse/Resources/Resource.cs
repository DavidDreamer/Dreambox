using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Omniverse
{
	public class Resource
	{
		public AsyncReactiveProperty<float> Capacity { get; }
		
		public AsyncReactiveProperty<float> Amount { get; }
		
		public AsyncReactiveProperty<float> Regeneration { get; }
		
		public  bool Vital { get; }
		
		public bool Invulnerable { get; private set; }

		public bool OutOf => Amount.Value == 0;
		
		public Resource(ResourceDescriptor descriptor)
		{
			Capacity = new AsyncReactiveProperty<float>(descriptor.Capacity);
			Amount = new AsyncReactiveProperty<float>(descriptor.Capacity);
			Regeneration = new AsyncReactiveProperty<float>(descriptor.Regeneration);

			Vital = descriptor.Vital;
		}
		
		public void FixedTick()
		{
			Change(Regeneration.Value * Time.fixedDeltaTime);
		}

		public void Restore()
		{
			Amount.Value = Capacity.Value;
		}

		public void Change(float delta)
		{
			Debug.Assert(!Invulnerable);

			Amount.Value = Mathf.Clamp(Amount.Value + delta, 0, Capacity.Value);
		}
	}
}
