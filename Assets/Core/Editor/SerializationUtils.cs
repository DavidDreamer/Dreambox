namespace Dreambox.Core.Editor
{
	public static class SerializationUtils
	{
		public static string ToBackingField(this string propertyName) => $"<{propertyName}>k__BackingField";
	}
}
