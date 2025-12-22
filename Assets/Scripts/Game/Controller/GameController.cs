using System.Collections.Generic;
using Game.Model;
using Game.Model.Data;
using Game.View;
using UnityEngine;

namespace Game.Controller
{
	public class GameController
	{
		private GameField _gameField;
		private GameFieldView _gameFieldView;

		public void Init(
			GameFieldView gameFieldView, 
			TileViewLibrary tileViewLibrary, 
			TileViewPool tileViewPool, 
			int rowsCount, 
			int columnsCount,
			int tileWidth,
			int tileHeight,
			int gameFieldWidth,
			int gameFieldHeight)
		{
			_gameField = new GameField();
			_gameField.Init(rowsCount, columnsCount);
			
			_gameFieldView = gameFieldView;
			_gameFieldView.Init(
				_gameField, 
				tileViewLibrary,
				tileViewPool,
				tileWidth,
				tileHeight,
				gameFieldWidth,
				gameFieldHeight);
			_gameFieldView.OnTileClickRequested += HandleTileClick;
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
		}
	}
}