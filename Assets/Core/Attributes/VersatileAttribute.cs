using System;
using UnityEngine;

namespace Dreambox.Core
{
	public class VersatileAttribute: PropertyAttribute
	{
		public Type Type { get; }

		public VersatileAttribute(Type type)
		{
			Type = type;
		}
	}
}
