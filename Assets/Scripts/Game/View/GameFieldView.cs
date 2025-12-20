using Game.Model;
using UnityEngine;

namespace Game.View
{
	public class GameFieldView : MonoBehaviour
	{
		[SerializeField] private RectTransform tilesParent;
		
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

		private void FillField()
		{
			for (var x = 0; x < _gameField.Tiles.GetLength(0); x++)
			{
				for (var y = 0; y < _gameField.Tiles.GetLength(1); y++)
				{
					TileModel tile = _gameField.Tiles[x, y];
					Sprite sprite = _tileViewLibrary.GetSprite(tile.Color);
					CreateTile(sprite, tile.X, tile.Y);
				}
			}
		}

		private void CreateTile(Sprite sprite, int x, int y)
		{
			TileView tileView = _tileViewPool.GetTile();
			tileView.transform.SetParent(tilesParent, false);
			tileView.SetSprite(sprite);
			tileView.RectTransform.anchoredPosition = CalculateTilePosition(x, y);
			_tiles[x, y] = tileView;
		}

		private Vector2 CalculateTilePosition(int x, int y)
		{
			return new Vector2(_tileWidth * y, _tileHeight * x);
		}
	}
}