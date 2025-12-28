using System.Collections.Generic;
using Game.Model.Data;
using Game.Model.SuperTile;
using UnityEngine;

namespace Game.Model
{
	public class TileFieldManager
	{
		private TileModel[,] _tiles;
		private TileModelPool _pool;
		private int _rowsCount;
		private int _columnsCount;

		public void Init(TileModel[,] tiles, TileModelPool pool, int rowsCount, int columnsCount)
		{
			_tiles = tiles;
			_pool = pool;
			_rowsCount = rowsCount;
			_columnsCount = columnsCount;
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

		public void SetColors(TileColor[,] colors)
		{
			for (var row = 0; row < _rowsCount; row++)
			for (var column = 0; column < _columnsCount; column++)
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

		public void ClearAllTiles()
		{
			for (var row = 0; row < _rowsCount; row++)
			for (var column = 0; column < _columnsCount; column++)
			{
				if (_tiles[row, column] != null)
				{
					_pool.ReturnTile(_tiles[row, column]);
					_tiles[row, column] = null;
				}
			}
		}
	}
}