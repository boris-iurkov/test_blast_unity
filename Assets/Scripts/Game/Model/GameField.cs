using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Model
{
	public class GameField
	{
		private TileColor[] _sourceColors;
		private TileModel[,] _tiles;

		public TileModel[,] Tiles => _tiles;
		public int RowsCount { get; private set; }
		public int ColumnsCount { get; private set; }

		public void Init(int rowsCount, int columnsCount)
		{
			RowsCount = rowsCount;
			ColumnsCount = columnsCount;
			
			InitSourceColors();
			InitStartTiles();
		}

		public List<Vector2Int> GetTileGroup(int row, int column)
		{
			var result = new List<Vector2Int>();
			var visited = new bool[RowsCount, ColumnsCount];
			
			TileModel startTile = _tiles[row, column];
			AddNeighborTiles(row, column, startTile.Color, visited, result);

			return result;
		}

		public void RemoveTileGroup(List<Vector2Int> group)
		{
			foreach (Vector2Int positions in group)
				_tiles[positions.x, positions.y] = null;
		}

		private void InitSourceColors()
		{
			_sourceColors = (TileColor[])Enum.GetValues(typeof(TileColor));
		}

		private void InitStartTiles()
		{
			_tiles = new TileModel[RowsCount, ColumnsCount];
			
			for (var row = 0; row < RowsCount; row++)
			{
				for (var column = 0; column < ColumnsCount; column++)
				{
					var tile = new TileModel();
					tile.SetColor(GetRandomTileColor());
					tile.SetPositions(row, column);

					_tiles[row, column] = tile;
				}
			}
		}

		private TileColor GetRandomTileColor()
		{
			int randomColorIndex = Random.Range(0, _sourceColors.Length);
			return _sourceColors[randomColorIndex];
		}

		private void AddNeighborTiles(int row, int column, TileColor targetColor, bool[,] visited, List<Vector2Int> result)
		{
			if (row < 0 || row >= RowsCount || column < 0 || column >= ColumnsCount)
				return;

			if (visited[row, column])
				return;

			TileModel tile = _tiles[row, column];
			
			if (tile == null)
				return;
			
			if (tile.Color != targetColor)
				return;

			visited[row, column] = true;
			result.Add(new Vector2Int(row, column));
			
			AddNeighborTiles(row, column - 1, targetColor, visited, result);
			AddNeighborTiles(row, column + 1, targetColor, visited, result);
			AddNeighborTiles(row - 1, column, targetColor, visited, result);
			AddNeighborTiles(row + 1, column, targetColor, visited, result);
		}
	}
}