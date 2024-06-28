using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dreambox.Core.Editor
{
	public static class TypeUtils
	{
		private static Dictionary<Type, List<Type>> InheritedTypes { get; } = new();

		public static List<Type> GetInheritedTypes(this Type sourceType, bool includeAbstract = false)
		{
			if (!InheritedTypes.ContainsKey(sourceType))
			{
				var types = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(assembly => assembly.GetTypes())
					.Where(type => type.IsClass && sourceType.IsAssignableFrom(type));

				if (includeAbstract is false)
				{
					types = types.Where(type => !type.IsAbstract);
				}

				var typesList = types.ToList();

				InheritedTypes.Add(sourceType, typesList);
			}

			return InheritedTypes[sourceType];
		}

		public static Type GetTypeFromFullName(string typeFullName)
		{
			string[] typeData = typeFullName.Split();
			string assemblyName = typeData[0];
			string typeName = typeData[1];
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
				.First(assemply => assemply.GetName().Name == assemblyName);
			Type type = assembly.GetType(typeName);
			return type;
		}
	}
}
