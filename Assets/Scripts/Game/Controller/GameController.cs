using Game.Model;
using Game.View;
using UnityEngine;

namespace Game.Controller
{
	public class GameController
	{
		private GameField _gameField;
		private GameFieldView _gameFieldView;

		public void Init(
			GameFieldView gameFieldView, 
			TileViewLibrary tileViewLibrary, 
			TileViewPool tileViewPool, 
			int rowsCount, 
			int columnsCount,
			int tileWidth,
			int tileHeight)
		{
			_gameField = new GameField();
			_gameField.Init(rowsCount, columnsCount);
			
			_gameFieldView = gameFieldView;
			_gameFieldView.Init(
				_gameField, 
				tileViewLibrary,
				tileViewPool,
				tileWidth,
				tileHeight);
			_gameFieldView.OnTileClickRequested += HandleTileClick;
		}

		private void HandleTileClick(int row, int column)
		{
			TileModel tile = _gameField.GetTile(row, column);
		}
	}
}