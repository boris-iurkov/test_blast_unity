using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.View
{
	public class TileView : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] private Image image;
		[SerializeField] private RectTransform rectTransform;

		public RectTransform RectTransform => rectTransform;
		public event Action<int, int> Clicked;

		private int _row;
		private int _column;

		public void SetSprite(Sprite sprite)
		{
			image.sprite = sprite;
		}

		public void SetPositions(int row, int column)
		{
			_row = row;
			_column = column;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			Clicked?.Invoke(_row, _column);
		}
	}
}