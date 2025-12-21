using System;
using System.Collections.Generic;
using Game.Model;
using UnityEngine;

namespace Game.View
{
	public class GameFieldView : MonoBehaviour
	{
		[SerializeField] private RectTransform tilesParent;

		public event Action<int, int> OnTileClickRequested;
		
		private GameField _gameField;
		private TileViewLibrary _tileViewLibrary;
		private TileView[,] _tiles;
		private TileViewPool _tileViewPool;

		private int _tileWidth;
		private int _tileHeight;

		public void Init(
			GameField gameField, 
			TileViewLibrary tileViewLibrary,
			TileViewPool tileViewPool,
			int tileWidth,
			int tileHeight)
		{
			_gameField = gameField;
			_tileViewLibrary = tileViewLibrary;
			_tileViewPool = tileViewPool;
			_tileWidth = tileWidth;
			_tileHeight = tileHeight;
			
			_tiles = new TileView[_gameField.RowsCount, _gameField.ColumnsCount];
			
			FillField();
		}

		public void RemoveTileGroup(List<Vector2Int> group)
		{
			foreach (Vector2Int positions in group)
			{
				TileView tile = _tiles[positions.x, positions.y];
				tile.SetClickable(false);
				_tileViewPool.ReturnTile(tile);
				_tiles[positions.x, positions.y] = null;
			}
		}

		private void FillField()
		{
			int rows = _gameField.Tiles.GetLength(0);
			int columns = _gameField.Tiles.GetLength(1);
			
			for (var row = 0; row < rows; row++)
			{
				for (var column = 0; column < columns; column++)
				{
					TileModel tile = _gameField.Tiles[row, column];
					Sprite sprite = _tileViewLibrary.GetSprite(tile.Color);
					CreateTile(sprite, tile.Row, tile.Column);
				}
			}
		}

		private void CreateTile(Sprite sprite, int row, int column)
		{
			TileView tileView = _tileViewPool.GetTile();
			tileView.transform.SetParent(tilesParent, false);
			tileView.RectTransform.anchoredPosition = CalculateTilePosition(row, column);
			
			tileView.SetSprite(sprite);
			tileView.SetPositions(row, column);

			tileView.Clicked += OnTileClicked;
			
			_tiles[row, column] = tileView;
		}

		private Vector2 CalculateTilePosition(int row, int column)
		{
			return new Vector2(_tileWidth * column, _tileHeight * row);
		}

		private void OnTileClicked(int row, int column)
		{
			OnTileClickRequested?.Invoke(row, column);
		}
	}
}