using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
	public class TileView : MonoBehaviour
	{
		[SerializeField] private Image image;

		public void SetSprite(Sprite sprite)
		{
			image.sprite = sprite;
		}
	}
}