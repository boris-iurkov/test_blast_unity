using System.Collections.Generic;
using Game.Model.Data;
using UnityEngine;

namespace Game.Model
{
	public class TileFallCalculator
	{
		private TileModel[,] _tiles;
		private int _rowsCount;
		private int _columnsCount;

		public void Init(TileModel[,] tiles, int rowsCount, int columnsCount)
		{
			_tiles = tiles;
			_rowsCount = rowsCount;
			_columnsCount = columnsCount;
		}

		public List<TileFallData> CalculateFalls()
		{
			var result = new List<TileFallData>();

			for (var column = 0; column < _columnsCount; column++)
			{
				var emptyTiles = 0;
				
				for (var row = 0; row < _rowsCount; row++)
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
			}
			
			return result;
		}

		public List<TileFallData> GetAllTilesFallData()
		{
			var result = new List<TileFallData>();
			
			for (var row = 0; row < _rowsCount; row++)
			for (var column = 0; column < _columnsCount; column++)
			{
				TileModel tile = _tiles[row, column];
				if (tile != null)
				{
					int spawnRow = _rowsCount + row + 1;
						
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

		public int GetEmptyTilesCountInColumn(int column)
		{
			var emptyCount = 0;
			for (var row = 0; row < _rowsCount; row++)
			{
				if (_tiles[row, column] == null)
					emptyCount++;
			}
			return emptyCount;
		}
	}
}