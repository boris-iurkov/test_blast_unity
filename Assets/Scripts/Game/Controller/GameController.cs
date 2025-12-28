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
		private BoosterPanelView _boosterSwapView;
		private BoosterPanelView _boosterBombView;
		private EndGamePopup _endGamePopup;
		
		private BoosterSwapController _boosterSwapController;
		private ViewController _viewController;
		private ShuffleController _shuffleController;
		private EndGameController _endGameController;

		private InteractionMode _interactionMode = InteractionMode.Common;
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
			_gameFieldView.SwapTilesCompleted += HandleSwapTilesCompleted;

			_boosterSwapView = boosterSwapView;
			_boosterSwapView.Init();
			_boosterSwapView.OnBoosterClicked += HandleBoosterSwapClicked;
			
			_boosterBombView = boosterBombView;
			_boosterBombView.Init();
			_boosterBombView.OnBoosterClicked += HandleBoosterBombClicked;

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
			_boosterSwap.OnBoosterUsed += HandleBoosterSwapUsed;

			_boosterBomb = new BoosterBombCounter();
			_boosterBomb.Init(gameConfigData.BoosterBombStartCount, gameConfigData.BoosterBombRadius);
			_boosterBomb.OnBoosterUsed += HandleBoosterBombUsed;

			_superTileFactory = new SuperTileFactory();

			_boosterSwapController = new BoosterSwapController();
			_boosterSwapController.OnTileSelected += HandleTileSelected;
			_boosterSwapController.OnTileUnselected += HandleTileUnselected;

			_viewController = new ViewController();
			_viewController.Init(movesView, scoreView, boosterSwapView, boosterBombView, _movesCounter, _scoreCounter, _boosterSwap, _boosterBomb);
			_viewController.UpdateAllViews();
		}

		private void HandleTileClick(int row, int column)
		{
			if (_endGameController.IsEndGame
			    || _shuffleController.IsShuffling
			    || _boosterSwapController.IsSwapping
			    || _gameFieldView.IsTileFalling(row, column))
				return;

			bool isSuperTile = _gameField.Tiles[row, column].SuperLogic != null;
			List<Vector2Int> group;
			switch (_interactionMode)
			{
				case InteractionMode.BoosterSwap:
					_boosterSwapController.OnTileClicked(_gameField.Tiles[row, column]);
					return;

				case InteractionMode.BoosterBomb:
					group = _gameField.GetBoosterBombTileGroup(row, column, _boosterBomb.Radius);
					break;
				
				default:
					if (isSuperTile)
						group = _gameField.GetSuperTileGroup(row, column);
					else
						group = _gameField.GetCommonTileGroup(row, column);
					break;
			}

			var groupWithoutFallingTiles = new List<Vector2Int>();
			foreach (Vector2Int pos in group)
			{
				if (!_gameFieldView.IsTileFalling(pos.x, pos.y))
					groupWithoutFallingTiles.Add(pos);
			}
			group = groupWithoutFallingTiles;
			int groupCount = group.Count;

			if (_interactionMode == InteractionMode.Common)
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

			if (_interactionMode == InteractionMode.BoosterBomb)
				_scoreCounter.AddScoreForBomb(groupCount);
			else
			{
				if (isSuperTile)
					_scoreCounter.AddScoreForSuperTile(groupCount);
				else
					_scoreCounter.AddScoreForGroup(groupCount);
			}
			_movesCounter.MakeMove();
			
			if (_interactionMode == InteractionMode.BoosterBomb)
				_boosterBomb.Use();

			_interactionMode = InteractionMode.Common;
			
			UpdateSelectionsByInteractionMode();
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
		
		private void HandleBoosterSwapClicked()
		{
			if (_boosterSwap.Count <= 0)
				return;
			
			_interactionMode = _interactionMode != InteractionMode.BoosterSwap
				? InteractionMode.BoosterSwap
				: InteractionMode.Common;

			UpdateSelectionsByInteractionMode();
		}
		
		private void HandleBoosterSwapUsed()
		{
			_viewController.UpdateBoosterSwapView();
		}

		private void HandleBoosterBombClicked()
		{
			if (_boosterBomb.Count <= 0)
				return;
			
			_interactionMode = _interactionMode != InteractionMode.BoosterBomb
				? InteractionMode.BoosterBomb
				: InteractionMode.Common;
			
			UpdateSelectionsByInteractionMode();
		}
		
		private void HandleBoosterBombUsed()
		{
			_viewController.UpdateBoosterBombView();
		}

		private void UpdateSelectionsByInteractionMode()
		{
			switch (_interactionMode)
			{
				case InteractionMode.BoosterSwap:
					_viewController.UpdateBoosterSwapSelection(true);
					_viewController.UpdateBoosterBombSelection(false);
					break;
				
				case InteractionMode.BoosterBomb:
					_viewController.UpdateBoosterSwapSelection(false);
					_viewController.UpdateBoosterBombSelection(true);
					break;
				
				default:
					_viewController.UpdateBoosterSwapSelection(false);
					_viewController.UpdateBoosterBombSelection(false);
					break;
			}
			
			if (_interactionMode != InteractionMode.BoosterSwap)
			{
				TileModel firstTile = _boosterSwapController.FirstSelected;
				if (firstTile != null)
					_gameFieldView.UnselectTile(firstTile);
				_boosterSwapController.Reset();
			}
		}

		private void HandleTileSelected(TileModel tile)
		{
			_gameFieldView.SelectTile(tile);
			if (_boosterSwapController.SecondSelected != null)
			{
				TileModel firstTile = _boosterSwapController.FirstSelected;
				TileModel secondTile = _boosterSwapController.SecondSelected;

				_gameFieldView.UnselectTile(firstTile);
				_gameFieldView.UnselectTile(secondTile);
				
				_gameFieldView.SwapTiles(firstTile, secondTile);
				_gameField.SwapTiles(firstTile, secondTile);
				
				_boosterSwap.Use();
			}
		}
		
		private void HandleTileUnselected(TileModel tile)
		{
			_gameFieldView.UnselectTile(tile);
		}

		private void HandleSwapTilesCompleted()
		{
			_gameFieldView.UpdateTileLayers(_boosterSwapController.FirstSelected, _boosterSwapController.SecondSelected);
			
			_boosterSwapController.Reset();

			TileColor[,] colors = _gameFieldView.GetCurrentTileColors();
			_gameField.SetColors(colors);
			
			_boosterSwapController.Reset();
			_interactionMode = InteractionMode.Common;
			UpdateSelectionsByInteractionMode();
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
			
			_interactionMode = InteractionMode.Common;
			_boosterSwapController.Reset();
			_endGameController.Reset();
			_shuffleController.Reset();
			
			_viewController.UnselectAllBoosters();

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