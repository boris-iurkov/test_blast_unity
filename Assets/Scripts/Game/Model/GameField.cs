using System.Collections.Generic;
using Game.Model.Data;
using Game.Model.SuperTile;
using UnityEngine;

namespace Game.Model
{
	public class GameField
	{
		private TileModel[,] _tiles;
		private readonly TileModelPool _pool = new();
		private readonly GroupFinder _groupFinder = new();
		private readonly TileFallCalculator _tileFallCalculator = new();
		private readonly TileSpawner _tileSpawner = new();

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
			
			_tiles = new TileModel[RowsCount, ColumnsCount];
			
			_tileSpawner.Init(_pool, _tiles, RowsCount, ColumnsCount);
			_tileSpawner.SpawnStartTiles();
			
			_groupFinder.Init(_tiles, RowsCount, ColumnsCount, RadiusSuperTileBombSmall);
			
			_tileFallCalculator.Init(_tiles, RowsCount, ColumnsCount);
		}

		public List<Vector2Int> GetCommonTileGroup(int row, int column)
		{
			return _groupFinder.GetCommonTileGroup(row, column);
		}
		
		public List<Vector2Int> GetSuperTileGroup(int row, int column)
		{
			return _groupFinder.GetSuperTileGroup(row, column);
		}
		
		public List<Vector2Int> GetBoosterBombTileGroup(int row, int column, int radius)
		{
			return _groupFinder.GetBoosterBombTileGroup(row, column, radius);
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
			List<TileFallData> result = _tileFallCalculator.CalculateFalls();
			for (var column = 0; column < ColumnsCount; column++)
			{
				int emptyTiles = _tileFallCalculator.GetEmptyTilesCountInColumn(column);
				
				for (var i = 0; i < emptyTiles; i++)
				{
					int targetRow = RowsCount - emptyTiles + i;
					int spawnRow = RowsCount + i + 1;

					TileModel newTile = _tileSpawner.SpawnTile(targetRow, column);

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
			return _groupFinder.HasAnyAvailableGroup();
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
			
			_tileSpawner.SpawnStartTiles();
			_groupFinder.Init(_tiles, RowsCount, ColumnsCount, RadiusSuperTileBombSmall);
			_tileFallCalculator.Init(_tiles, RowsCount, ColumnsCount);
		}

		public List<TileFallData> GetAllTilesFallData()
		{
			return _tileFallCalculator.GetAllTilesFallData();
		}
	}
}