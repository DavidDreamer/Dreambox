using UnityEngine;

namespace Dreambox.Rendering.Universal
{
	public struct OutlineTarget
	{
		public Mesh Mesh { get; }

		public Material Material { get; }

		public Matrix4x4 Matrix { get; }

		public int Variant { get; }

		public OutlineTarget(Mesh mesh, Material material, Matrix4x4 matrix, int variant)
		{
			Mesh = mesh;
			Material = material;
			Matrix = matrix;
			Variant = variant;
		}
	}
}
