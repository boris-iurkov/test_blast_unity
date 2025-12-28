using System.Collections.Generic;
using DG.Tweening;
using Game.Controller.Controllers;
using Game.Controller.Data;
using Game.Controller.Factory;
using Game.Model;
using Game.Model.Booster;
using Game.Model.Data;
using Game.Model.SuperTile;
using Game.View;
using Game.View.Data;
using Game.View.Popup;
using Game.View.Tile;
using UnityEngine;

namespace Game.Controller
{
	public class GameController
	{
		private GameField _gameField;
		private MovesCounter _movesCounter;
		private ScoreCounter _scoreCounter;
		private BoosterCounter _boosterSwap;
		private BoosterBombCounter _boosterBomb;

		private GameFieldView _gameFieldView;
		private EndGamePopup _endGamePopup;
		
		private BoosterController _boosterController;
		private ViewController _viewController;
		private ShuffleController _shuffleController;
		private EndGameController _endGameController;
		private SuperTileFactory _superTileFactory;

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
			
			_gameFieldView.OnTileClickRequested += HandleTileClick;
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

			_superTileFactory = new SuperTileFactory();

			_viewController = new ViewController();
			_viewController.Init(movesView, scoreView, boosterSwapView, boosterBombView, _movesCounter, _scoreCounter, _boosterSwap, _boosterBomb);

			_boosterController = new BoosterController();
			_boosterController.Init(_boosterSwap, _boosterBomb, boosterSwapView, boosterBombView, _gameFieldView, _gameField, _viewController);
			_gameFieldView.SwapTilesCompleted += _boosterController.HandleSwapTilesCompleted;

			_viewController.UpdateAllViews();
			_boosterController.UpdateViews();
		}

		private void HandleTileClick(int row, int column)
		{
			if (_endGameController.IsEndGame
			    || _shuffleController.IsShuffling
			    || _boosterController.BoosterSwapController.IsSwapping
			    || _gameFieldView.IsTileFalling(row, column))
				return;

			if (_boosterController.InteractionMode == InteractionMode.BoosterSwap)
			{
				TileModel tile = _gameField.Tiles[row, column];
				if (tile != null)
					_boosterController.BoosterSwapController.OnTileClicked(tile);
				return;
			}

			bool isSuperTile = _gameField.Tiles[row, column].SuperLogic != null;
			List<Vector2Int> group;
			if (_boosterController.InteractionMode == InteractionMode.BoosterBomb)
			{
				group = _gameField.GetBoosterBombTileGroup(row, column, _boosterController.BoosterBomb.Radius);
			}
			else
			{
				if (isSuperTile)
					group = _gameField.GetSuperTileGroup(row, column);
				else
					group = _gameField.GetCommonTileGroup(row, column);
			}

			var groupWithoutFallingTiles = new List<Vector2Int>();
			foreach (Vector2Int pos in group)
			{
				if (!_gameFieldView.IsTileFalling(pos.x, pos.y))
					groupWithoutFallingTiles.Add(pos);
			}
			group = groupWithoutFallingTiles;
			int groupCount = group.Count;

			if (_boosterController.InteractionMode == InteractionMode.Common)
			{
				if (groupCount < 2)
					return;

				if (groupCount >= _gameField.MinSuperTileGroupSize && !isSuperTile)
				{
					group.RemoveAll(tile => tile.x == row && tile.y == column);
					ISuperTileLogic superTileLogic = _superTileFactory.CreateRandomSuperTile();
					_gameField.SetSuperTileLogic(row, column, superTileLogic);
					_gameFieldView.UpdateTileView(row, column, _gameField.Tiles[row, column].Color);
				}
			}
			
			List<Vector2Int> finalGroup = ActivateSuperTilesInGroup(group);
			
			_gameField.RemoveTileGroup(finalGroup);
			_gameFieldView.RemoveTileGroup(finalGroup, row, column);

			float destroyDuration = _gameFieldView.GetDestroyGroupDuration(finalGroup, new Vector2Int(row, column), 0.05f);
			DOVirtual.DelayedCall(destroyDuration, () =>
			{
				List<TileFallData> fallingTiles = _gameField.ApplyFallTiles();
				_gameFieldView.FallTiles(fallingTiles);
			});

			if (_boosterController.InteractionMode == InteractionMode.BoosterBomb)
				_scoreCounter.AddScoreForBomb(groupCount);
			else
			{
				if (isSuperTile)
					_scoreCounter.AddScoreForSuperTile(groupCount);
				else
					_scoreCounter.AddScoreForGroup(groupCount);
			}
			_movesCounter.MakeMove();
			
			if (_boosterController.InteractionMode == InteractionMode.BoosterBomb)
				_boosterController.BoosterBomb.Use();

			_boosterController.Reset();
		}

		private List<Vector2Int> ActivateSuperTilesInGroup(List<Vector2Int> group)
		{
			var finalGroup = new HashSet<Vector2Int>(group);
			
			var superTilesToActivate = new List<Vector2Int>();
			foreach (Vector2Int pos in group)
			{
				TileModel tile = _gameField.Tiles[pos.x, pos.y];
				if (tile != null && tile.SuperLogic != null)
					superTilesToActivate.Add(pos);
			}
			
			foreach (Vector2Int superTilePos in superTilesToActivate)
			{
				List<Vector2Int> superTileGroup = _gameField.GetSuperTileGroup(superTilePos.x, superTilePos.y);
				foreach (Vector2Int affectedPos in superTileGroup)
					finalGroup.Add(affectedPos);
			}
			
			return new List<Vector2Int>(finalGroup);
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