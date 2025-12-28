using System.Collections.Generic;
using DG.Tweening;
using Game.Controller.Controllers;
using Game.Model;
using Game.Model.Booster;
using Game.Model.Data;
using Game.View;

namespace Game.Controller
{
	public class GameController
	{
		private IGameField _gameField;
		private IMovesCounter _movesCounter;
		private IScoreCounter _scoreCounter;
		private IBoosterCounter _boosterSwap;
		private IBoosterBombCounter _boosterBomb;
		private IGameFieldView _gameFieldView;
		
		private ITileInteractionController _tileInteractionController;
		private IBoosterController _boosterController;
		private IViewController _viewController;
		private IShuffleController _shuffleController;
		private IEndGameController _endGameController;

		public void Init(
			IGameField gameField,
			IGameFieldView gameFieldView,
			IMovesCounter movesCounter,
			IScoreCounter scoreCounter,
			IBoosterCounter boosterSwap,
			IBoosterBombCounter boosterBomb,
			IEndGameController endGameController,
			IShuffleController shuffleController,
			IViewController viewController,
			IBoosterController boosterController,
			ITileInteractionController tileInteractionController)
		{
			_gameField = gameField;
			_gameFieldView = gameFieldView;
			_movesCounter = movesCounter;
			_scoreCounter = scoreCounter;
			_boosterSwap = boosterSwap;
			_boosterBomb = boosterBomb;
			_endGameController = endGameController;
			_shuffleController = shuffleController;
			_viewController = viewController;
			_boosterController = boosterController;
			_tileInteractionController = tileInteractionController;
			
			_movesCounter.OnMovesChanged += HandleMovesChanged;
			_scoreCounter.OnScoreChanged += HandleScoreChanged;
			_endGameController.OnRestartRequested += RestartGame;
			_gameFieldView.OnTileClickRequested += OnTileClickRequested;
			_gameFieldView.FallCompleted += HandleFallCompleted;
			_gameFieldView.SwapTilesCompleted += _boosterController.HandleSwapTilesCompleted;
		}

		private void OnTileClickRequested(int row, int column)
		{
			_tileInteractionController.HandleTileClick(row, column, _endGameController.IsEndGame, _shuffleController.IsShuffling);
		}

		private void HandleMovesChanged()
		{
			_viewController.UpdateMovesView();
			_endGameController.TryEndGame();
		}
		
		private void HandleScoreChanged()
		{
			_viewController.UpdateScoreView();
			_endGameController.TryEndGame();
		}
		
		private void HandleFallCompleted()
		{
			if (_gameFieldView.HasFallingTiles())
				return;
			
			_endGameController.TryEndGame();
			_shuffleController.TryEndGameByShuffle();
			
			if (_endGameController.IsEndGame)
				_endGameController.ShowEndGamePopup();
		}

		private void RestartGame()
		{
			DOTween.KillAll();
			
			_endGameController.Reset();
			_shuffleController.Reset();
			_boosterController.Reset();

			_movesCounter.Init(_movesCounter.MaxMoves);
			_scoreCounter.Init(_scoreCounter.TargetScore);
			_boosterBomb.Init(_boosterBomb.StartCount, _boosterBomb.Radius);
			_boosterSwap.Init(_boosterSwap.StartCount);

			_viewController.UpdateAllViews();
			
			_gameFieldView.ClearAllTiles();
			_gameField.Reset();
			
			List<TileFallData> fallTiles = _gameField.GetAllTilesFallData();
			_gameFieldView.FallTiles(fallTiles);
		}
	}
}