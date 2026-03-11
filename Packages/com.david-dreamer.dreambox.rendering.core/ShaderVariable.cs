using System.Runtime.CompilerServices;
using UnityEngine;

namespace Dreambox.Rendering.Core
{
	public static class ShaderVariable
	{
		public static int Create([CallerMemberName] string name = "") => Shader.PropertyToID($"_{name}");
	}
}
