using Cysharp.Threading.Tasks;
using Dreambox.Core;
using Dreambox.Core.Pooling;
using Dreambox.Physics;
using UnityEngine;
using UnityEngine.AI;

namespace Omniverse
{
	public class UnitPresenter: MonoBehaviour, IPoolObject
	{
		[field: SerializeField]
		public NavMeshAgent NavMeshAgent { get; private set; }

		[field: SerializeField]
		public Animator Animator { get; set; }

		[field: SerializeField]
		public Collider Hitbox { get; private set; }

		[field: SerializeField]
		private Ragdoll Ragdoll { get; set; }

		[field: SerializeField]
		public AudioSource AudioSource { get; private set; }
		
		[field: SerializeField]
		[field: HideInInspector]
		public Renderer[] Renderers { get; private set; }

		[field: SerializeField]
		[field: HideInInspector]
		private IDeathHandler[] DeathHandlers { get; set; }

		public Unit Unit { get; private set; }

		public UniTaskCompletionSource UniTaskCompletionSource { get; set; } = new();

		private void OnValidate()
		{
			Renderers = GetComponentsInChildren<Renderer>(true);
			DeathHandlers = GetComponentsInChildren<IDeathHandler>();
		}

		public void Bind(Unit unit)
		{
			Unit = unit;

			if (NavMeshAgent != null)
			{
				NavMeshAgent.speed = Unit.Desc.Stats.MovementSpeed;
			}
		}

		public void Cleanup()
		{
			UniTaskCompletionSource = new UniTaskCompletionSource();

			Unit = null;
			
			Hitbox.enabled = true;
			Animator.enabled = true;
			
			if (NavMeshAgent != null)
			{
				NavMeshAgent.enabled = true;
			}

			if (Ragdoll != null)
			{
				Ragdoll.Enable(true);
			}
		}

		public void Tick()
		{
			NavMeshAgent.isStopped = Unit.Status.HasFlag(UnitStatus.Stunned);
		}
		
		public void AddForce(Vector3 force)
		{
			if (Ragdoll != null)
			{
				Ragdoll.AddForce(force);
			}
		}

		public void Despawn()
		{
			UniTaskCompletionSource.TrySetResult();
		}

		public virtual void OnDeath()
		{
			Hitbox.enabled = false;
			Animator.enabled = false;
			NavMeshAgent.enabled = false;

			Ragdoll.Enable(true);
			
			if (DeathHandlers != null)
			{
				foreach (IDeathHandler deathHandler in DeathHandlers)
				{
					deathHandler.OnDead(this);
				}
			}

			UniTaskCompletionSource.TrySetResult();
		}

		public void UpdateMovementFromNavMeshAgent()
		{
			Animator.SetFloat(AnimatorParameter.Get("MovementSpeed"), NavMeshAgent.velocity.magnitude);
		}
		
		public void MoveDirection(Vector3 direction)
		{
			Animator.SetFloat(AnimatorParameter.Get("MovementSpeed"), direction != Vector3.zero ? 1f : 0f);

			if (direction != Vector3.zero)
			{
				float dotProduct = Vector3.Dot(transform.forward, direction);

				if (dotProduct <= 0)
				{
					transform.forward = direction;
				}
				else
				{
					float delta = Unit.Desc.Stats.AngularSpeed * Time.deltaTime;
					transform.forward = Vector3.RotateTowards(transform.forward, direction, delta, 1f);
				}
			}

			Vector3 velocity = direction * Unit.Desc.Stats.MovementSpeed * Time.deltaTime;
			Vector3 nextPosition = transform.position + velocity;

			if (NavMesh.SamplePosition(nextPosition, out NavMeshHit hit, NavMeshAgent.height * 2, 1))
			{
				transform.position = hit.position;
			}
		}
	}
}
