using Game.Model;
using Game.Model.Booster;
using Game.View;

namespace Game.Controller.Controllers
{
	public class ViewController
	{
		private MovesView _movesView;
		private ScoreView _scoreView;
		private BoosterPanelView _boosterSwapView;
		private BoosterPanelView _boosterBombView;
		
		private MovesCounter _movesCounter;
		private ScoreCounter _scoreCounter;
		private BoosterCounter _boosterSwap;
		private BoosterBombCounter _boosterBomb;

		public void Init(
			MovesView movesView,
			ScoreView scoreView,
			BoosterPanelView boosterSwapView,
			BoosterPanelView boosterBombView,
			MovesCounter movesCounter,
			ScoreCounter scoreCounter,
			BoosterCounter boosterSwap,
			BoosterBombCounter boosterBomb)
		{
			_movesView = movesView;
			_scoreView = scoreView;
			_boosterSwapView = boosterSwapView;
			_boosterBombView = boosterBombView;
			
			_movesCounter = movesCounter;
			_scoreCounter = scoreCounter;
			_boosterSwap = boosterSwap;
			_boosterBomb = boosterBomb;
		}

		public void UpdateMovesView()
		{
			_movesView.UpdateMovesCount(_movesCounter.MovesLeft);
		}

		public void UpdateScoreView()
		{
			_scoreView.UpdateScoreCount(_scoreCounter.Score, _scoreCounter.TargetScore);
		}

		public void UpdateBoosterSwapView()
		{
			_boosterSwapView.UpdateCount(_boosterSwap.Count);
		}

		public void UpdateBoosterBombView()
		{
			_boosterBombView.UpdateCount(_boosterBomb.Count);
		}

		public void UpdateAllViews()
		{
			UpdateMovesView();
			UpdateScoreView();
			UpdateBoosterSwapView();
			UpdateBoosterBombView();
		}

		public void UpdateBoosterSwapSelection(bool isSelected)
		{
			if (isSelected)
				_boosterSwapView.Select();
			else
				_boosterSwapView.Unselect();
		}

		public void UpdateBoosterBombSelection(bool isSelected)
		{
			if (isSelected)
				_boosterBombView.Select();
			else
				_boosterBombView.Unselect();
		}

		public void UnselectAllBoosters()
		{
			_boosterSwapView.Unselect();
			_boosterBombView.Unselect();
		}
	}
}