using UnityEngine;

namespace Omniverse
{
	public class UnitPresenter: MonoBehaviour
	{
		public Unit Unit { get; set; }
		
		[field: SerializeField]
		public  Animator Animator { get; private set; }
		
		public virtual void OnDeath()
		{
		}

		public virtual void AddForce(Vector3 force)
		{
		}

		public virtual void Tick()
		{
		}
	}
}
