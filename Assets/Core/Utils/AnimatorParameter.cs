using System.Collections.Generic;
using UnityEngine;

namespace Dreambox
{
	public static class AnimatorParameter
	{
		private static Dictionary<string, int> Cache { get; } = new();

		public static int Get(string name)
		{
			if (!Cache.ContainsKey(name))
			{
				int id = Animator.StringToHash(name);

				Cache.Add(name, id);
			}

			return Cache[name];
		}
	}
}