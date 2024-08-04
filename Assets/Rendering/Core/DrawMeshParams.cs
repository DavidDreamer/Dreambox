using System;
using UnityEngine;

namespace Dreambox.Rendering.Core
{
	[Serializable]
	public class DrawMeshParams
	{
		[field: SerializeField]
		public Mesh Mesh { get; private set; }

		[field: SerializeField]
		public int SubmeshIndex { get; private set; }

		[field: SerializeField]
		public Material Material { get; private set; }

		[field: SerializeField]
		public int ShaderPass { get; private set; }
	}
}
