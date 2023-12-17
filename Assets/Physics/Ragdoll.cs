using UnityEngine;

namespace Dreambox.Physics
{
	public class Ragdoll: MonoBehaviour
	{
		[field: SerializeField]
		private Rigidbody Pelvis { get; set; }

		[field: SerializeField]
		[field: HideInInspector]
		private Rigidbody[] Rigidbodies { get; set; }

		private void OnValidate()
		{
			Rigidbodies = GetComponentsInChildren<Rigidbody>(true);
		}

		public void Enable(bool value)
		{
			foreach (Rigidbody body in Rigidbodies)
			{
				body.isKinematic = !value;
			}
		}

		public void AddForce(Vector3 force)
		{
			foreach (Rigidbody body in Rigidbodies)
			{
				body.AddForce(force, ForceMode.Impulse);
			}
		}
	}
}
