using DG.Tweening;
using Game.Controller.Data;
using Game.Model;
using Game.Model.Data;
using Game.View;

namespace Game.Controller.Controllers
{
	public class ShuffleController
	{
		public bool IsShuffling { get; private set; }
		
		private GameField _gameField;
		private GameFieldView _gameFieldView;
		private EndGameController _endGameController;
		private int _maxShuffles;
		private int _countShufflesMade;

		public void Init(GameField gameField, GameFieldView gameFieldView, EndGameController endGameController, int maxShuffles)
		{
			_gameField = gameField;
			_gameFieldView = gameFieldView;
			_endGameController = endGameController;
			_maxShuffles = maxShuffles;
			
			_gameFieldView.ShuffleCompleted += HandleShuffleCompleted;
		}
		
		public void TryEndGameByShuffle()
		{
			ShuffleResult shuffleResult = TryShuffle();
			if (shuffleResult == ShuffleResult.MaxShuffles)
			{
				_endGameController.SetShufflesCount(_countShufflesMade);
				_endGameController.TryEndGame();
			}
			
			if (_endGameController.IsEndGame)
				_endGameController.ShowEndGamePopup();
		}

		public void Reset()
		{
			_countShufflesMade = 0;
			IsShuffling = false;
		}

		private ShuffleResult TryShuffle()
		{
			ShuffleResult shuffleResult = GetShuffleResult();
			if (shuffleResult == ShuffleResult.NeedShuffle)
			{
				IsShuffling = true;
				_countShufflesMade++;
				DOVirtual.DelayedCall(1f, () =>
				{
					_gameFieldView.ShuffleTiles();
				});
			}

			return shuffleResult;
		}

		private ShuffleResult GetShuffleResult()
		{
			if (_endGameController.IsEndGame)
				return ShuffleResult.EndGame;

			if (_gameField.HasAnyAvailableGroup())
				return ShuffleResult.HasGroup;

			if (_countShufflesMade < _maxShuffles)
				return ShuffleResult.NeedShuffle;

			return ShuffleResult.MaxShuffles;
		}

		private void HandleShuffleCompleted()
		{
			TileColor[,] colors = _gameFieldView.GetCurrentTileColors();
			_gameField.SetColors(colors);

			IsShuffling = false;
			_endGameController.Reset();

			TryEndGameByShuffle();
		}
	}
}