using Game.Model;
using Game.View;

namespace Game.Controller
{
	public class GameController
	{
		private Board _board;
		private BoardView _boardView;

		public void Init(BoardView boardView)
		{
			_board = new Board();
			_board.Init();
			
			_boardView = boardView;
			_boardView.Init(_board);
		}
	}
}