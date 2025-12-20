using Game.Model;
using Game.View;

namespace Game.Controller
{
	public class GameController
	{
		private GameField _gameField;
		private GameFieldView _gameFieldView;

		public void Init(GameFieldView gameFieldView)
		{
			_gameField = new GameField();
			_gameField.Init();
			
			_gameFieldView = gameFieldView;
			_gameFieldView.Init(_gameField);
		}
	}
}