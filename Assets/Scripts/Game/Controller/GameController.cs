using Game.Core;

namespace Game.Controller
{
	public class GameController
	{
		private Board _board;

		public void Init()
		{
			_board = new Board();
			_board.Init();
		}
	}
}