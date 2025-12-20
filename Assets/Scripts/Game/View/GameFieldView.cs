using Game.Model;
using UnityEngine;

namespace Game.View
{
	public class GameFieldView : MonoBehaviour
	{
		private TileView[,] _tiles;
		private GameField _gameField;

		public void Init(GameField gameField)
		{
			_gameField = gameField;
			_tiles = new TileView[_gameField.Width, _gameField.Height];
		}
	}
}