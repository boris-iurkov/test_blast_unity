using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
	public class TileView : MonoBehaviour
	{
		[SerializeField] private Image image;
		[SerializeField] private RectTransform rectTransform;

		public RectTransform RectTransform => rectTransform;

		public void SetSprite(Sprite sprite)
		{
			image.sprite = sprite;
		}
	}
}