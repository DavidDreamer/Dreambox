using UnityEngine;

namespace Dreambox.Rendering.Core
{
	public struct OutlineRenderer
	{
		public Mesh Mesh { get; }

		public Material Material { get; }

		public Matrix4x4 Matrix { get; }

		public int Variant { get; }

		public OutlineRenderer(Mesh mesh, Material material, Matrix4x4 matrix, int variant)
		{
			Mesh = mesh;
			Material = material;
			Matrix = matrix;
			Variant = variant;
		}
	}
}
