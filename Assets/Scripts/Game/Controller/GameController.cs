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
			
			_movesCounter = new MovesCounter();
			_movesCounter.OnMovesChanged += HandleMovesChanged;
			_movesCounter.Init(gameConfigData.MaxMoves);

			_scoreCounter = new ScoreCounter();
			_scoreCounter.OnScoreChanged += HandleScoreChanged;
			_scoreCounter.Init(gameConfigData.TargetScore);

			_maxShuffles = gameConfigData.MaxShuffles;
		}

		private void HandleTileClick(int row, int column)
		{
			if (_isEndGame)
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
			
			_scoreCounter.AddScoreForGroup(group.Count);
			
			_gameField.RemoveTileGroup(group);
			_gameFieldView.RemoveTileGroup(group);

			List<TileFallData> fallTiles = _gameField.ApplyFallTiles();
			_gameFieldView.FallTiles(fallTiles);

			_movesCounter.MakeMove();
		}

		private void HandleMovesChanged(int movesLeft)
		{
			_gameFieldView.UpdateMovesCount(movesLeft);
		}

		private void HandleScoreChanged(int score, int targetScore)
		{
			_gameFieldView.UpdateScoreCount(score, targetScore);
		}

		private void CheckEndGame()
		{
			if (_scoreCounter.Score >= _scoreCounter.TargetScore)
			{
				SetEndGame(EndGameResult.Win);
				return;
			}

			if (_movesCounter.MovesLeft <= 0)
			{
				SetEndGame(EndGameResult.LoseNoMoves);
				return;
			}

			if (!_gameField.HasAnyAvailableGroup())
			{
				if (_countShufflesMade < _maxShuffles)
				{
					DOVirtual.DelayedCall(1f, () =>
					{
						_countShufflesMade++;
						Shuffle();
					});
				}
				else
					SetEndGame(EndGameResult.LoseNoTiles);
				return;
			}

			_isEndGame = false;
			_endGameResult = EndGameResult.None;
		}

		private void SetEndGame(EndGameResult result)
		{
			_isEndGame = true;
			_endGameResult = result;
		}

		private void HandleFallCompleted()
		{
			CheckEndGame();
			
			if (!_isEndGame)
				return;
			
			ShowEndGamePopup();
		}

		private void HandleShuffleCompleted()
		{
			TileColor[,] colors = _gameFieldView.GetCurrentTileColors();
			_gameField.SetColors(colors);
			
			_isEndGame = false;
			
			CheckEndGame();
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
			
			_gameFieldView.ShuffleTiles();
		}

		private void Shuffle()
		{
			_gameFieldView.ShuffleTiles();
		}
	}
}