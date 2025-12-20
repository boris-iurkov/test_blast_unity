using Game.Model;
using UnityEngine;

namespace Game.View
{
	public class GameFieldView : MonoBehaviour
	{
		private GameField _gameField;
		private TileView[,] _tiles;
		private TileViewPool _tileViewPool;

		public void Init(GameField gameField, TileViewPool tileViewPool)
		{
			_gameField = gameField;
			_tiles = new TileView[_gameField.Width, _gameField.Height];
			_tileViewPool = tileViewPool;
		}
	}
}