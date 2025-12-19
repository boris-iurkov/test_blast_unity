using Game.Model;
using UnityEngine;

namespace Game.View
{
	public class BoardView : MonoBehaviour
	{
		private TileView[,] _tiles;
		private Board _board;

		public void Init(Board board)
		{
			_board = board;
			_tiles = new TileView[_board.Width, _board.Height];
		}
	}
}