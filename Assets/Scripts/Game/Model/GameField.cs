using System.Collections.Generic;
using Game.Model.Data;
using Game.Model.SuperTile;
using UnityEngine;

namespace Game.Model
{
	public class GameField : IGameField
	{
		private TileModel[,] _tiles;
		private readonly TileModelPool _pool = new();
		private readonly GroupFinder _groupFinder = new();
		private readonly TileFallCalculator _tileFallCalculator = new();
		private readonly TileSpawner _tileSpawner = new();
		private readonly TileFieldManager _tileFieldManager = new();

		public int RowsCount { get; private set; }
		public int ColumnsCount { get; private set; }
		public int MinSuperTileGroupSize { get; private set; }
		public int RadiusSuperTileBombSmall { get; private set; }
		
		public TileModel GetTile(int row, int column)
		{
			return _tiles[row, column];
		}
		
		public void SetTile(int row, int column, TileModel tile)
		{
			_tiles[row, column] = tile;
		}
		
		public bool IsSuperTile(int row, int column)
		{
			TileModel tile = GetTile(row, column);
			return tile != null && tile.SuperLogic != null;
		}

		public void Init(GameConfigData configData)
		{
			RowsCount = configData.RowsCount;
			ColumnsCount = configData.ColumnsCount;
			MinSuperTileGroupSize = configData.MinSuperTileGroupSize;
			RadiusSuperTileBombSmall = configData.RadiusSuperTileBombSmall;
			
			_tiles = new TileModel[RowsCount, ColumnsCount];
			
			_tileFieldManager.Init(_tiles, _pool, RowsCount, ColumnsCount);
			
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
			_tileFieldManager.RemoveTileGroup(group);
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
			_tileFieldManager.SetColors(colors);
		}

		public void SwapTiles(TileModel tile1, TileModel tile2)
		{
			_tileFieldManager.SwapTiles(tile1, tile2);
		}

		public void SetSuperTileLogic(int row, int column, ISuperTileLogic superTileLogic)
		{
			_tileFieldManager.SetSuperTileLogic(row, column, superTileLogic);
		}

		public void Reset()
		{
			_tileFieldManager.ClearAllTiles();
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