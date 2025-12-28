using System.Collections.Generic;
using Game.Model.Data;
using Game.Model.SuperTile;
using UnityEngine;

namespace Game.Model
{
	public interface IGameField
	{
		TileModel[,] Tiles { get; }
		int RowsCount { get; }
		int ColumnsCount { get; }
		int MinSuperTileGroupSize { get; }
		int RadiusSuperTileBombSmall { get; }
		
		void Init(GameConfigData configData);
		List<Vector2Int> GetCommonTileGroup(int row, int column);
		List<Vector2Int> GetSuperTileGroup(int row, int column);
		List<Vector2Int> GetBoosterBombTileGroup(int row, int column, int radius);
		void RemoveTileGroup(List<Vector2Int> group);
		List<TileFallData> ApplyFallTiles();
		bool HasAnyAvailableGroup();
		void SetColors(TileColor[,] colors);
		void SwapTiles(TileModel tile1, TileModel tile2);
		void SetSuperTileLogic(int row, int column, ISuperTileLogic superTileLogic);
		void Reset();
		List<TileFallData> GetAllTilesFallData();
	}
}