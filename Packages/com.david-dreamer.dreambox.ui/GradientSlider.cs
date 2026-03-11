using UnityEngine;
using UnityEngine.UI;

namespace Dreambox.UI
{
	[ExecuteAlways]
	[RequireComponent(typeof(Slider))]
	public class GradientSlider : MonoBehaviour
	{
		[field: SerializeField]
		private Gradient Gradient { get; set; }

		[field: SerializeField]
		[field: HideInInspector]
		private Slider Slider { get; set; }

		[field: SerializeField]
		[field: HideInInspector]
		private Image FillImage { get; set; }

		private void OnValidate()
		{
			Slider = GetComponent<Slider>();
			FillImage = Slider.fillRect.GetComponent<Image>();
		}

		private void LateUpdate()
		{
			FillImage.color = Gradient.Evaluate(Slider.normalizedValue);
		}
	}
}
