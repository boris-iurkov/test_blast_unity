using System.Collections.Generic;
using DG.Tweening;
using Game.Controller.Controllers;
using Game.Controller.Factory;
using Game.Model;
using Game.Model.Booster;
using Game.Model.Data;
using Game.View;
using Game.View.Data;
using Game.View.Popup;
using Game.View.Tile;

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
		private EndGamePopup _endGamePopup;
		
		private TileInteractionController _tileInteractionController;
		private BoosterController _boosterController;
		private ViewController _viewController;
		private ShuffleController _shuffleController;
		private EndGameController _endGameController;

		public void Init(
			GameFieldView gameFieldView,
			MovesView movesView,
			ScoreView scoreView,
			BoosterPanelView boosterSwapView,
			BoosterPanelView boosterBombView,
			EndGamePopup endGamePopup,
			TileViewLibrary tileViewLibrary,
			TileViewPool tileViewPool, 
			GameConfigData gameConfigData,
			FieldConfigData fieldConfigData)
		{
			_gameField = new GameField();
			_gameField.Init(gameConfigData);
			
			_gameFieldView = gameFieldView;
			_gameFieldView.Init(
				tileViewLibrary,
				tileViewPool,
				fieldConfigData,
				_gameField.RowsCount,
				_gameField.ColumnsCount);
			
			for(var row = 0; row < _gameField.RowsCount; row++)
			for (var column = 0; column < _gameField.ColumnsCount; column++)
				_gameFieldView.FillTile(_gameField.Tiles[row, column]);
			
			_gameFieldView.OnTileClickRequested += (row, column) => 
				_tileInteractionController.HandleTileClick(row, column, _endGameController.IsEndGame, _shuffleController.IsShuffling);
			_gameFieldView.FallCompleted += HandleFallCompleted;

			_endGamePopup = endGamePopup;

			_movesCounter = new MovesCounter();
			_movesCounter.Init(gameConfigData.MaxMoves);
			_movesCounter.OnMovesChanged += HandleMovesChanged;
			
			_scoreCounter = new ScoreCounter();
			_scoreCounter.Init(gameConfigData.TargetScore);
			_scoreCounter.OnScoreChanged += HandleScoreChanged;
			
			_endGameController = new EndGameController();
			_endGameController.Init(_scoreCounter, _movesCounter, _endGamePopup, gameConfigData.MaxShuffles);
			_endGameController.OnRestartRequested += RestartGame;

			_shuffleController = new ShuffleController();
			_shuffleController.Init(_gameField, _gameFieldView, _endGameController, gameConfigData.MaxShuffles);

			_boosterSwap = new BoosterCounter();
			_boosterSwap.Init(gameConfigData.BoosterSwapStartCount);

			_boosterBomb = new BoosterBombCounter();
			_boosterBomb.Init(gameConfigData.BoosterBombStartCount, gameConfigData.BoosterBombRadius);

			_viewController = new ViewController();
			_viewController.Init(movesView, scoreView, boosterSwapView, boosterBombView, _movesCounter, _scoreCounter, _boosterSwap, _boosterBomb);

			_boosterController = new BoosterController();
			_boosterController.Init(_boosterSwap, _boosterBomb, boosterSwapView, boosterBombView, _gameFieldView, _gameField, _viewController);
			_gameFieldView.SwapTilesCompleted += _boosterController.HandleSwapTilesCompleted;

			var superTileFactory = new SuperTileFactory();
			_tileInteractionController = new TileInteractionController();
			_tileInteractionController.Init(_gameField, _gameFieldView, _scoreCounter, _movesCounter, _boosterController, superTileFactory);

			_viewController.UpdateAllViews();
			_boosterController.UpdateViews();
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