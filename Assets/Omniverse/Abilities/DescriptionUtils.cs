namespace Omniverse
{
	public static class DescriptionUtils
	{
		public static Ability Build(this AbilityDescription description, Unit unit) => new(description, unit);

		public static Resource Build(this ResourceDescriptor description) => new(description);
	}
}
