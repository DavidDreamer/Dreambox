using System.Collections.Generic;
using Dreambox.Core.Editor;
using UnityEditor;

namespace Omniverse.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(AbilityDescription))]
	public class AbilityEditor: OverridableInspector
	{
		protected override IEnumerable<string> CustomProperties
		{
			get
			{
				yield return nameof(AbilityDescription.Target).ToBackingField();
				yield return nameof(AbilityDescription.Cooldown).ToBackingField();
			}
		}
		
		protected override void DrawCustomProperty(SerializedProperty property)
		{
			if (property.name == nameof(AbilityDescription.Target).ToBackingField())
			{
				property.DrawManagedReferenceFromInterface(typeof(ITarget));
				return;
			}
			
			if (property.name == nameof(AbilityDescription.Cooldown).ToBackingField())
			{
				property.DrawManagedReference(typeof(CooldownDescriptor));
			}
		}
	}
}
