namespace Game.Controller.Controllers
{
	public interface ITileInteractionController
	{
		void HandleTileClick(int row, int column, bool isEndGame, bool isShuffling);
	}
}