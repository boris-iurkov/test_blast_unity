using System;
using System.Collections.Generic;
using System.Linq;
using Game.Model.Data;
using Game.Model.SuperTile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Model
{
	public class GameField
	{
		private TileColor[] _sourceColors;
		private TileModel[,] _tiles;
		private readonly TileModelPool _pool = new();

		public TileModel[,] Tiles => _tiles;
		public int RowsCount { get; private set; }
		public int ColumnsCount { get; private set; }
		public int MinSuperTileGroupSize { get; private set; }
		public int RadiusSuperTileBombSmall { get; private set; }

		public void Init(GameConfigData configData)
		{
			RowsCount = configData.RowsCount;
			ColumnsCount = configData.ColumnsCount;
			MinSuperTileGroupSize = configData.MinSuperTileGroupSize;
			RadiusSuperTileBombSmall = configData.RadiusSuperTileBombSmall;
			
			InitSourceColors();
			InitStartTiles();
		}

		public List<Vector2Int> GetCommonTileGroup(int row, int column)
		{
			var result = new List<Vector2Int>();
			var visited = new bool[RowsCount, ColumnsCount];
			
			TileModel startTile = _tiles[row, column];
			AddNeighborTiles(row, column, startTile.Color, visited, result);

			return result;
		}
		
		public List<Vector2Int> GetSuperTileGroup(int row, int column)
		{
			ISuperTileLogic logic = _tiles[row, column].SuperLogic;
			if (logic is SuperTileLogicExplodeSmall explodeSmall)
				explodeSmall.Init(RadiusSuperTileBombSmall);
			
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

		public void RemoveTileGroup(List<Vector2Int> group)
		{
			foreach (Vector2Int positions in group)
			{
				TileModel tile = _tiles[positions.x, positions.y];
				if (tile != null)
				{
					_pool.ReturnTile(tile);
					_tiles[positions.x, positions.y] = null;
				}
			}
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
						
						tile.SetPositions(to.x, to.y);

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

					TileModel newTile = _pool.GetTile();
					newTile.SetColor(GetRandomTileColor());
					newTile.SetPositions(targetRow, column);
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

		public void SetColors(TileColor[,] colors)
		{
			for (var row = 0; row < RowsCount; row++)
			for (var column = 0; column < ColumnsCount; column++)
				_tiles[row, column].SetColor(colors[row, column]);
		}

		public void SwapTiles(TileModel tile1, TileModel tile2)
		{
			int row1 = tile1.Row;
			int column1 = tile1.Column;
			int row2 = tile2.Row;
			int column2 = tile2.Column;
			
			TileModel firstTile = _tiles[row1, column1];
			TileModel secondTile = _tiles[row2, column2];
			
			(_tiles[row1, column1], _tiles[row2, column2]) = (_tiles[row2, column2], _tiles[row1, column1]);
			
			firstTile.SetPositions(row2, column2);
			secondTile.SetPositions(row1, column1);
		}

		public void SetSuperTileLogic(int row, int column, ISuperTileLogic superTileLogic)
		{
			TileModel tile = _tiles[row, column];
			tile.SetSuperTileLogic(superTileLogic);
			tile.SetColor(superTileLogic.TileColor);
		}

		public void Reset()
		{
			for (var row = 0; row < RowsCount; row++)
			for (var column = 0; column < ColumnsCount; column++)
			{
				if (_tiles[row, column] != null)
				{
					_pool.ReturnTile(_tiles[row, column]);
					_tiles[row, column] = null;
				}
			}
			
			InitStartTiles();
		}

		public List<TileFallData> GetAllTilesFallData()
		{
			var result = new List<TileFallData>();
			
			for (var row = 0; row < RowsCount; row++)
			for (var column = 0; column < ColumnsCount; column++)
			{
				TileModel tile = _tiles[row, column];
				if (tile != null)
				{
					int spawnRow = RowsCount + row + 1;
						
					result.Add(new TileFallData
					{
						Tile = tile,
						From = new Vector2Int(spawnRow, column),
						To = new Vector2Int(row, column)
					});
				}
			}
			
			return result;
		}

		private bool IsTileInsideField(int row, int column)
		{
			return row >= 0 &&
			       row < RowsCount &&
			       column >= 0 &&
			       column < ColumnsCount;
		}

		private void InitSourceColors()
		{
			_sourceColors = Enum.GetValues(typeof(TileColor))
				.Cast<TileColor>()
				.Take(5)
				.ToArray();
		}

		private void InitStartTiles()
		{
			_tiles = new TileModel[RowsCount, ColumnsCount];
			
			for (var row = 0; row < RowsCount; row++)
			{
				for (var column = 0; column < ColumnsCount; column++)
				{
					TileModel tile = _pool.GetTile();
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