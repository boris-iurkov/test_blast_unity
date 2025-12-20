using System.Collections.Generic;
using UnityEngine;

namespace Game.View
{
	public class TileViewPool : MonoBehaviour
	{
		[SerializeField] private TileView tilePrefab;
		[SerializeField] private int initialSize;
		[SerializeField] private RectTransform tilesContainer;

		private Stack<TileView> _stack;

		private void Awake()
		{
			_stack = new Stack<TileView>(initialSize);
			
			for (var i = 0; i < initialSize; i++)
			{
				TileView tile = CreateNewTile();
				tile.gameObject.SetActive(false);
				_stack.Push(tile);
			}
		}

		public TileView GetTile()
		{
			TileView tile = _stack.Count > 0 ? _stack.Pop() : CreateNewTile();
			tile.gameObject.SetActive(true);
			return tile;
		}

		public void ReturnTile(TileView tile)
		{
			tile.gameObject.SetActive(false);
			_stack.Push(tile);
		}
		
		private TileView CreateNewTile()
		{
			TileView tile = Instantiate(tilePrefab, tilesContainer);
			tile.gameObject.SetActive(false);
			return tile;
		}
	}
}