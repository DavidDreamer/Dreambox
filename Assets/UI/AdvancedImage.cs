using UnityEngine;
using UnityEngine.UI;

namespace Dreambox.UI
{
	[AddComponentMenu("UI/Advanced Image", 12)]
	public class AdvancedImage : Image
	{
		[field: SerializeField]
		public Color ColorLeftBottom { get; private set; } = Color.white;

		[field: SerializeField]
		public Color ColorLeftTop { get; private set; } = Color.white;

		[field: SerializeField]
		public Color ColorRightBottom { get; private set; } = Color.white;

		[field: SerializeField]
		public Color ColorRightTop { get; private set; } = Color.white;

		protected override void OnPopulateMesh(VertexHelper vertexHelper)
		{
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			Vector4 vector = new(pixelAdjustedRect.x, pixelAdjustedRect.y, pixelAdjustedRect.x + pixelAdjustedRect.width, pixelAdjustedRect.y + pixelAdjustedRect.height);
			vertexHelper.Clear();
			vertexHelper.AddVert(new Vector3(vector.x, vector.y), ColorLeftBottom, new Vector2(0f, 0f));
			vertexHelper.AddVert(new Vector3(vector.x, vector.w), ColorLeftTop, new Vector2(0f, 1f));
			vertexHelper.AddVert(new Vector3(vector.z, vector.w), ColorRightBottom, new Vector2(1f, 1f));
			vertexHelper.AddVert(new Vector3(vector.z, vector.y), ColorRightTop, new Vector2(1f, 0f));
			vertexHelper.AddTriangle(0, 1, 2);
			vertexHelper.AddTriangle(2, 3, 0);
		}
	}
}
