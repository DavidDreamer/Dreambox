using System.Text.RegularExpressions;

namespace Dreambox.Core.Editor
{
	public static class StringUtils
	{
		public static string SplitCamelCaseWithSpaces(this string source) => Regex.Replace(source, "(\\B[A-Z])", " $1");
	}
}
