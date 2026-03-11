using System.Runtime.CompilerServices;
using UnityEngine;

namespace Dreambox.Rendering.Core
{
	public static class ShaderVariable
	{
		public static int Create([CallerMemberName] string name = null) => Shader.PropertyToID($"_{name}");
	}
}
