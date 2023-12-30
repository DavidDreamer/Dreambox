using System.Collections.Generic;
using Dreambox.Core.Editor;
using Omniverse.Abilities.Description;
using UnityEditor;

namespace Omniverse.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Ability))]
	public class AbilityEditor: OverridableInspector
	{
		protected override IEnumerable<string> CustomProperties
		{
			get
			{
				yield return nameof(Ability.Cooldown).ToBackingField();
			}
		}
		
		protected override void DrawCustomProperty(SerializedProperty property)
		{
			if (property.name == nameof(Ability.Cooldown).ToBackingField())
			{
				property.DrawManagedReference(typeof(Cooldown));
			}
		}
	}
}
