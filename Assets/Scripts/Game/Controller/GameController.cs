using Game.Model;
using Game.View;

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
				tileViewPool,
				tileWidth,
				tileHeight);
		}
	}
}