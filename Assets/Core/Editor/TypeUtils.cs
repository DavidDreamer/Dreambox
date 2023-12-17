using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambox.Core.Editor
{
	public static class TypeUtils
	{
		private static Dictionary<Type, List<Type>> InheritedTypes { get; } = new();

		public static List<Type> GetInheritedTypes(this Type type)
		{
			if (!InheritedTypes.ContainsKey(type))
			{
				var types = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(assembly => assembly.GetTypes())
					.Where(t => t.IsClass && type.IsAssignableFrom(t)).ToList();

				InheritedTypes.Add(type, types);
			}

			return InheritedTypes[type];
		}
	}
}
