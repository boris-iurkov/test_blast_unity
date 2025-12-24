using System.Collections.Generic;
using DG.Tweening;
using Game.Controller.Data;
using Game.Model;
using Game.Model.Booster;
using Game.Model.Data;
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
		private MovesView _movesView;
		private ScoreView _scoreView;
		private BoosterPanelView _boosterSwapView;
		private BoosterPanelView _boosterBombView;
		private EndGamePopup _endGamePopup;

		private bool _isEndGame = false;
		private EndGameResult _endGameResult = EndGameResult.None;
		
		private int _maxShuffles;
		private int _countShufflesMade = 0;
		private bool _isShuffling = false;

		private InteractionMode _interactionMode = InteractionMode.Common;

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
				_gameField, 
				tileViewLibrary,
				tileViewPool,
				fieldConfigData);
			_gameFieldView.OnTileClickRequested += HandleTileClick;
			_gameFieldView.FallCompleted += HandleFallCompleted;
			_gameFieldView.ShuffleCompleted += HandleShuffleCompleted;

			_boosterSwapView = boosterSwapView;
			_boosterSwapView.Init();
			_boosterSwapView.OnBoosterClicked += HandleBoosterSwapClicked;
			
			_boosterBombView = boosterBombView;
			_boosterBombView.Init();
			_boosterBombView.OnBoosterClicked += HandleBoosterBombClicked;

			_endGamePopup = endGamePopup;
			_endGamePopup.gameObject.SetActive(false);
			_endGamePopup.OnButtonClicked += HandleEndGamePopupButtonClicked;

			_movesCounter = new MovesCounter();
			_movesCounter.Init(gameConfigData.MaxMoves);
			_movesCounter.OnMovesChanged += HandleMovesChanged;
			
			_scoreCounter = new ScoreCounter();
			_scoreCounter.Init(gameConfigData.TargetScore);
			_scoreCounter.OnScoreChanged += HandleScoreChanged;

			_boosterSwap = new BoosterCounter();
			_boosterSwap.Init(gameConfigData.BoosterSwapStartCount);

			_boosterBomb = new BoosterBombCounter();
			_boosterBomb.Init(gameConfigData.BoosterBombStartCount, gameConfigData.BoosterBombRadius);
			_boosterBomb.OnBoosterUsed += HandleBoosterBombUsed;
			
			_movesView = movesView;
			_scoreView = scoreView;

			_maxShuffles = gameConfigData.MaxShuffles;
			
			UpdateMovesView();
			UpdateScoreView();
			UpdateBoosterSwapView();
			UpdateBoosterBombView();
		}

		private void HandleTileClick(int row, int column)
		{
			if (_isEndGame)
				return;

			if (_isShuffling)
				return;

			if (_gameFieldView.IsTileFalling(row, column))
				return;
			
			List<Vector2Int> group;
			switch (_interactionMode)
			{
				case InteractionMode.BoosterBomb:
					group = _gameField.GetBoosterBombTileGroup(row, column, _boosterBomb.Radius);
					break;
				
				default:
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

			if (_interactionMode == InteractionMode.Common && group.Count < 2)
				return;

			_gameField.RemoveTileGroup(group);
			_gameFieldView.RemoveTileGroup(group);

			List<TileFallData> fallingTiles = _gameField.ApplyFallTiles();
			_gameFieldView.FallTiles(fallingTiles);

			_scoreCounter.AddScoreForGroup(group.Count);
			_movesCounter.MakeMove();

			if (_interactionMode == InteractionMode.BoosterBomb)
			{
				_boosterBomb.Use();
				_boosterBombView.Unselect();
				_interactionMode = InteractionMode.Common;
			}
		}

		private void HandleMovesChanged()
		{
			UpdateMovesView();
			TryEndGame();
		}

		private void UpdateMovesView()
		{
			_movesView.UpdateMovesCount(_movesCounter.MovesLeft);
		}
		
		private void HandleScoreChanged()
		{
			UpdateScoreView();
			TryEndGame();
		}

		private void UpdateScoreView()
		{
			_scoreView.UpdateScoreCount(_scoreCounter.Score, _scoreCounter.TargetScore);
		}
		
		private void HandleBoosterSwapClicked()
		{
			_interactionMode = _interactionMode != InteractionMode.BoosterSwap
				? InteractionMode.BoosterSwap
				: InteractionMode.Common;
		}

		private void UpdateBoosterSwapView()
		{
			_boosterSwapView.UpdateCount(_boosterSwap.Count);
		}
		
		private void HandleBoosterBombClicked()
		{
			if (_boosterBomb.Count <= 0)
				return;
			
			_interactionMode = _interactionMode != InteractionMode.BoosterBomb
				? InteractionMode.BoosterBomb
				: InteractionMode.Common;

			if (_interactionMode == InteractionMode.BoosterBomb)
				_boosterBombView.Select();
			else
				_boosterBombView.Unselect();
		}
		
		private void UpdateBoosterBombView()
		{
			_boosterBombView.UpdateCount(_boosterBomb.Count);
		}

		private void HandleBoosterBombUsed()
		{
			UpdateBoosterBombView();
		}
		
		private void TryEndGame()
		{
			if (_isEndGame)
				return;
			
			if (_scoreCounter.Score >= _scoreCounter.TargetScore)
			{
				_isEndGame = true;
				_endGameResult = EndGameResult.Win;
				return;
			}
			
			if (_movesCounter.MovesLeft <= 0)
			{
				_isEndGame = true;
				_endGameResult = EndGameResult.LoseNoMoves;
				return;
			}

			if (_countShufflesMade >= _maxShuffles)
			{
				_isEndGame = true;
				_endGameResult = EndGameResult.LoseNoTiles;
			}
		}

		private void HandleFallCompleted()
		{

			if (_gameFieldView.HasFallingTiles())
				return;
			
			TryEndGameByShuffle();

			if (_isEndGame)
				ShowEndGamePopup();
		}

		private void TryEndGameByShuffle()
		{
			ShuffleResult shuffleResult = TryShuffle();
			if (shuffleResult == ShuffleResult.MaxShuffles)
				TryEndGame();
		}

		private ShuffleResult TryShuffle()
		{
			ShuffleResult shuffleResult = GetShuffleResult();

			if (shuffleResult == ShuffleResult.NeedShuffle)
			{
				_isShuffling = true;
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
			if (_isEndGame)
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

			_isShuffling = false;
			_isEndGame = false;

			TryEndGameByShuffle();
		}

		private void ShowEndGamePopup()
		{
			_endGamePopup.gameObject.SetActive(true);
			EndGamePopupState state = GetEndGamePopupState(_endGameResult);
			_endGamePopup.Show(state);
		}

		private EndGamePopupState GetEndGamePopupState(EndGameResult endGameResult)
		{
			switch (endGameResult)
			{
				case EndGameResult.Win:
					return EndGamePopupState.Win;
				
				case EndGameResult.LoseNoMoves:
					return EndGamePopupState.LoseNoMoves;
				
				case EndGameResult.LoseNoTiles:
					return EndGamePopupState.LoseNoTiles;
				
				default:
					return EndGamePopupState.Win;
			}
		}
		
		private void HandleEndGamePopupButtonClicked()
		{
			_endGamePopup.Hide(() =>
			{
				_endGamePopup.gameObject.SetActive(false);
				RestartGame();
			});
		}

		private void RestartGame()
		{
			_endGameResult = EndGameResult.None;
			_countShufflesMade = 0;

			_movesCounter.Init(_movesCounter.MaxMoves);
			_scoreCounter.Init(_scoreCounter.TargetScore);
			_boosterBomb.Init(_boosterBomb.StartCount, _boosterBomb.Radius);
			_boosterSwap.Init(_boosterSwap.StartCount);

			UpdateScoreView();
			UpdateMovesView();
			UpdateBoosterSwapView();
			UpdateBoosterBombView();
			
			_isShuffling = true;
			_gameFieldView.ShuffleTiles();
		}
	}
}