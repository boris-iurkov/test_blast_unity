using System;
using System.Collections.Generic;
using Game.Model;
using Game.Model.Data;
using Game.View.Data;
using Game.View.Tile;
using UnityEngine;

namespace Game.View
{
	public interface IGameFieldView
	{
		event Action<int, int> OnTileClickRequested;
		event Action FallCompleted;
		event Action ShuffleCompleted;
		event Action SwapTilesCompleted;
		
		void Init(
			TileViewLibrary tileViewLibrary,
			TileViewPool tileViewPool,
			FieldConfigData configData,
			int rowsCount,
			int columnsCount);
		
		void FillTile(TileModel tile);
		void ClearAllTiles();
		void RemoveTileGroup(List<Vector2Int> group, int centerRow, int centerColumn);
		float GetDestroyGroupDuration(List<Vector2Int> group, Vector2Int origin, float stepDelay);
		void FallTiles(List<TileFallData> fallTiles);
		void ShuffleTiles();
		void UpdateTileLayers(TileModel tile1, TileModel tile2);
		void UpdateTileView(int row, int column, TileColor tileColor);
		TileColor[,] GetCurrentTileColors();
		bool IsTileFalling(int row, int column);
		bool HasFallingTiles();
		void SelectTile(TileModel tile);
		void UnselectTile(TileModel tile);
		void SwapTiles(TileModel tile1, TileModel tile2);
	}
}