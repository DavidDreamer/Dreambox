using System;

namespace Omniverse.Actions
{
	public static class ActionDescUtils
	{
		public static IAction Build(this IActionDesc actionDesc)
		{
			return actionDesc switch
			{
				AddTargetSelfDesc desc => new AddTargetSelf(desc),
				AnimationTriggerDesc desc => new AnimationTrigger(desc),
				ApplyEffectDesc desc => new ApplyEffect(desc),
				ApplyForceDesc desc => new ApplyForce(desc),
				ChangeResourceDesc desc => new ChangeResource(desc),
				ClearUnitsDesc desc => new ClearUnits(desc),
				CollectUnitTargetsFromSectorDesc desc => new CollectUnitTargetsFromSector(desc),
				CollectUnitTargetsFromSphereDesc desc => new CollectUnitTargetsFromSphere(desc),
				DelayDesc desc => new Delay(desc),
				LaunchProjectileDesc desc => new LaunchProjectile(desc),
				MoveInDirectionDesc desc => new MoveInDirection(desc),
				MoveToTargetDesc desc => new MoveToTarget(desc),
				SpawnVisualEffectDesc desc => new SpawnVisualEffect(desc),
				SuicideDesc desc => new Suicide(desc),
				_ => throw new ArgumentOutOfRangeException(nameof(actionDesc))
			};
		}
	}
}
