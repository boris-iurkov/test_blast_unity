using System.Collections.Generic;
using DG.Tweening;
using Game.Controller.Data;
using Game.Model;
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
		private MovesCounter _movesCounter;
		private ScoreCounter _scoreCounter;
		private GameField _gameField;
		private GameFieldView _gameFieldView;
		private EndGamePopup _endGamePopup;

		private bool _isEndGame = false;
		private EndGameResult _endGameResult = EndGameResult.None;
		private int _countShufflesMade = 0;
		private int _maxShuffles;
		private bool _isShuffling = false;

		public void Init(
			GameFieldView gameFieldView, 
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

			_endGamePopup = endGamePopup;
			_endGamePopup.gameObject.SetActive(false);
			_endGamePopup.OnButtonClicked += HandleEndGamePopupButtonClicked;

			_scoreCounter = new ScoreCounter();
			_scoreCounter.Init(gameConfigData.TargetScore);
			_scoreCounter.OnScoreChanged += HandleScoreChanged;
			
			_movesCounter = new MovesCounter();
			_movesCounter.Init(gameConfigData.MaxMoves);
			_movesCounter.OnMovesChanged += HandleMovesChanged;

			_maxShuffles = gameConfigData.MaxShuffles;
			
			UpdateScoreView(_scoreCounter.Score, _scoreCounter.TargetScore);
			UpdateMovesView(_movesCounter.MovesLeft);
		}

		private void HandleTileClick(int row, int column)
		{
			if (_isEndGame)
				return;

			if (_isShuffling)
				return;

			if (_gameFieldView.IsTileFalling(row, column))
				return;
			
			List<Vector2Int> group = _gameField.GetTileGroup(row, column);
			
			var groupWithoutFallingTiles = new List<Vector2Int>();
			foreach (Vector2Int pos in group)
			{
				if (!_gameFieldView.IsTileFalling(pos.x, pos.y))
					groupWithoutFallingTiles.Add(pos);
			}
			group = groupWithoutFallingTiles;

			if (group.Count < 2)
				return;

			_gameField.RemoveTileGroup(group);
			_gameFieldView.RemoveTileGroup(group);

			List<TileFallData> fallingTiles = _gameField.ApplyFallTiles();
			_gameFieldView.FallTiles(fallingTiles);

			_scoreCounter.AddScoreForGroup(group.Count);
			_movesCounter.MakeMove();
		}
		
		private void HandleScoreChanged(int score, int targetScore)
		{
			UpdateScoreView(score, targetScore);
			TryEndGame();
		}

		private void UpdateScoreView(int score, int targetScore)
		{
			_gameFieldView.UpdateScoreCount(score, targetScore);
		}

		private void HandleMovesChanged(int movesLeft)
		{
			UpdateMovesView(movesLeft);
			TryEndGame();
		}

		private void UpdateMovesView(int movesLeft)
		{
			_gameFieldView.UpdateMovesCount(movesLeft);
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

			_movesCounter.Reset();
			_scoreCounter.Reset();
			
			UpdateScoreView(_scoreCounter.Score, _scoreCounter.TargetScore);
			UpdateMovesView(_movesCounter.MovesLeft);
			
			_isShuffling = true;
			_gameFieldView.ShuffleTiles();
		}
	}
}