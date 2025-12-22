using System.Collections.Generic;
using Game.Model;
using Game.Model.Data;
using Game.View;
using Game.View.Data;
using UnityEngine;

namespace Game.Controller
{
	public class GameController
	{
		private MovesCounter _movesCounter;
		private GameField _gameField;
		private GameFieldView _gameFieldView;

		public void Init(
			GameFieldView gameFieldView, 
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
			
			_movesCounter = new MovesCounter();
			_movesCounter.OnMovesChanged += HandleMovesChanged;
			_movesCounter.Init(gameConfigData.MaxMoves);
		}

		private void HandleTileClick(int row, int column)
		{
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

			List<TileFallData> fallTiles = _gameField.ApplyFallTiles();
			_gameFieldView.FallTiles(fallTiles);

			_movesCounter.MakeMove();
		}

		private void HandleMovesChanged(int movesLeft)
		{
			_gameFieldView.UpdateMovesCount(movesLeft);
		}
	}
}