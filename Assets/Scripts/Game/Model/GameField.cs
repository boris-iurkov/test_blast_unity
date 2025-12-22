using System;
using System.Collections.Generic;
using Game.Model.Data;
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

		public void Init(GameConfigData configData)
		{
			RowsCount = configData.RowsCount;
			ColumnsCount = configData.ColumnsCount;
			
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

		public List<TileFallData> ApplyFallTiles()
		{
			var result = new List<TileFallData>();

			for (var column = 0; column < ColumnsCount; column++)
			{
				var emptyTiles = 0;
				
				for (var row = 0; row < RowsCount; row++)
				{
					if (_tiles[row, column] == null)
					{
						emptyTiles++;
						continue;
					}

					if (emptyTiles > 0)
					{
						TileModel tile = _tiles[row, column];
						
						var from = new Vector2Int(row, column);
						var to = new Vector2Int(row - emptyTiles, column);

						_tiles[row, column] = null;
						_tiles[to.x, to.y] = tile;

						result.Add(new TileFallData
						{
							Tile = tile, 
							From = from, 
							To = to
						});
					}
				}
				
				for (var i = 0; i < emptyTiles; i++)
				{
					int targetRow = RowsCount - emptyTiles + i;
					int spawnRow = RowsCount + i + 1;

					TileModel newTile = GetTileModel(targetRow, column);
					_tiles[targetRow, column] = newTile;

					result.Add(new TileFallData
					{
						Tile = newTile,
						From = new Vector2Int(spawnRow, column),
						To = new Vector2Int(targetRow, column)
					});
				}
			}
			
			return result;
		}
		
		public bool HasAnyAvailableGroup()
		{
			for (var row = 0; row < RowsCount; row++)
			{
				for (var col = 0; col < ColumnsCount; col++)
				{
					TileModel tile = _tiles[row, col];
					if (tile == null)
						continue;
					
					if (col + 1 < ColumnsCount)
					{
						TileModel right = _tiles[row, col + 1];
						if (right != null && right.Color == tile.Color)
							return true;
					}
					
					if (row + 1 < RowsCount)
					{
						TileModel up = _tiles[row + 1, col];
						if (up != null && up.Color == tile.Color)
							return true;
					}
				}
			}

			return false;
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
					TileModel tile = GetTileModel(row, column);
					_tiles[row, column] = tile;
				}
			}
		}

		private TileModel GetTileModel(int row, int column)
		{
			var tile = new TileModel();
			tile.SetColor(GetRandomTileColor());
			tile.SetPositions(row, column);
			return tile;
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