namespace Game.Controller.Controllers
{
	public interface IViewController
	{
		void UpdateMovesView();
		void UpdateScoreView();
		void UpdateBoosterSwapView();
		void UpdateBoosterBombView();
		void UpdateAllViews();
		void UpdateBoosterSwapSelection(bool isSelected);
		void UpdateBoosterBombSelection(bool isSelected);
		void UnselectAllBoosters();
	}
}