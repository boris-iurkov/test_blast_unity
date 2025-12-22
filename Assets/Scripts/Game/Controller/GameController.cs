using System.Collections.Generic;
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

			_endGamePopup = endGamePopup;
			_endGamePopup.gameObject.SetActive(false);
			_endGamePopup.OnButtonClicked += HandleEndGamePopupButtonClicked;
			
			_movesCounter = new MovesCounter();
			_movesCounter.OnMovesChanged += HandleMovesChanged;
			_movesCounter.Init(gameConfigData.MaxMoves);

			_scoreCounter = new ScoreCounter();
			_scoreCounter.OnScoreChanged += HandleScoreChanged;
			_scoreCounter.Init(gameConfigData.TargetScore);
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
			
			CheckEndGame();
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
				SetEndGame(EndGameResult.Lose);
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
			if (!_isEndGame)
				return;
			
			ShowEndGamePopup();
		}

		private void ShowEndGamePopup()
		{
			_endGamePopup.gameObject.SetActive(true);
			_endGamePopup.Show(_endGameResult == EndGameResult.Win);
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
			_isEndGame = false;
			_endGameResult = EndGameResult.None;
			
			_movesCounter.Reset();
			_scoreCounter.Reset();
		}
	}
}