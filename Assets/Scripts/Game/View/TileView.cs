using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
	public class TileView : MonoBehaviour
	{
		[SerializeField] private Image _image;

		public void SetSprite(Sprite sprite)
		{
			_image.sprite = sprite;
		}
	}
}