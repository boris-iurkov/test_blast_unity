using System.Collections.Generic;
using Game.Model.Data;
using Game.Model.SuperTile;
using UnityEngine;

namespace Game.Model
{
	public class GroupFinder
	{
		private TileModel[,] _tiles;
		private int _rowsCount;
		private int _columnsCount;
		private int _radiusSuperTileBombSmall;

		public void Init(TileModel[,] tiles, int rowsCount, int columnsCount, int radiusSuperTileBombSmall)
		{
			_tiles = tiles;
			_rowsCount = rowsCount;
			_columnsCount = columnsCount;
			_radiusSuperTileBombSmall = radiusSuperTileBombSmall;
		}

		public List<Vector2Int> GetCommonTileGroup(int row, int column)
		{
			var result = new List<Vector2Int>();
			var visited = new bool[_rowsCount, _columnsCount];
			
			TileModel startTile = _tiles[row, column];
			AddNeighborTiles(row, column, startTile.Color, visited, result);

			return result;
		}
		
		public List<Vector2Int> GetSuperTileGroup(int row, int column)
		{
			ISuperTileLogic logic = _tiles[row, column].SuperLogic;
			if (logic is SuperTileLogicExplodeSmall explodeSmall)
				explodeSmall.Init(_radiusSuperTileBombSmall);
			
			List<Vector2Int> result = logic.GetAffectedTiles(_tiles, new Vector2Int(row, column));
			return result;
		}
		
		public List<Vector2Int> GetBoosterBombTileGroup(int row, int column, int radius)
		{
			var result = new List<Vector2Int>();

			for (int r = row - radius; r <= row + radius; r++)
			for (int c = column - radius; c <= column + radius; c++)
			{
				if (!IsTileInsideField(r, c))
					continue;

				result.Add(new Vector2Int(r, c));
			}

			return result;
		}

		public bool HasAnyAvailableGroup()
		{
			for (var row = 0; row < _rowsCount; row++)
			{
				for (var col = 0; col < _columnsCount; col++)
				{
					TileModel tile = _tiles[row, col];
					if (tile == null)
						continue;
					
					if (col + 1 < _columnsCount)
					{
						TileModel right = _tiles[row, col + 1];
						if (right != null && right.Color == tile.Color)
							return true;
					}
					
					if (row + 1 < _rowsCount)
					{
						TileModel up = _tiles[row + 1, col];
						if (up != null && up.Color == tile.Color)
							return true;
					}
				}
			}

			return false;
		}

		private bool IsTileInsideField(int row, int column)
		{
			return row >= 0 &&
			       row < _rowsCount &&
			       column >= 0 &&
			       column < _columnsCount;
		}

		private void AddNeighborTiles(int row, int column, TileColor targetColor, bool[,] visited, List<Vector2Int> result)
		{
			if (row < 0 || row >= _rowsCount || column < 0 || column >= _columnsCount)
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