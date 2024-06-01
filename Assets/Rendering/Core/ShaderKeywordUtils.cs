using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Dreambox.Rendering.Core
{
	public static class ShaderKeywordUtils
	{
		public static void SwitchKeyword<T>(this Material material, T keyword)
		{
			Array enumValues = Enum.GetValues(typeof(T));

			var prefix = $"{Regex.Replace(typeof(T).Name, @"(?<=[a-z])([A-Z])", @"_$1").ToUpper()}_";

			foreach (object enumValue in enumValues)
			{
				var t = (T)enumValue;
				if (t.Equals(keyword))
				{
					material.EnableKeyword(prefix + enumValue.ToString().ToUpper());
				}
				else
				{
					material.DisableKeyword(prefix + enumValue.ToString().ToUpper());
				}
			}
		}
	}
}
