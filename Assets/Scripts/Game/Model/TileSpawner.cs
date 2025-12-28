using System;
using System.Linq;
using Game.Model.Data;
using Random = UnityEngine.Random;

namespace Game.Model
{
	public class TileSpawner
	{
		private TileColor[] _sourceColors;
		private TileModelPool _pool;
		private TileModel[,] _tiles;
		private int _rowsCount;
		private int _columnsCount;

		public void Init(TileModelPool pool, TileModel[,] tiles, int rowsCount, int columnsCount)
		{
			_pool = pool;
			_tiles = tiles;
			_rowsCount = rowsCount;
			_columnsCount = columnsCount;
			
			InitSourceColors();
		}

		public void SpawnStartTiles()
		{
			for (var row = 0; row < _rowsCount; row++)
			{
				for (var column = 0; column < _columnsCount; column++)
				{
					TileModel tile = _pool.GetTile();
					tile.SetColor(GetRandomTileColor());
					tile.SetPositions(row, column);
					_tiles[row, column] = tile;
				}
			}
		}

		public TileModel SpawnTile(int row, int column)
		{
			TileModel newTile = _pool.GetTile();
			newTile.SetColor(GetRandomTileColor());
			newTile.SetPositions(row, column);
			_tiles[row, column] = newTile;
			return newTile;
		}

		private void InitSourceColors()
		{
			_sourceColors = Enum.GetValues(typeof(TileColor))
				.Cast<TileColor>()
				.Take(5)
				.ToArray();
		}

		private TileColor GetRandomTileColor()
		{
			int randomColorIndex = Random.Range(0, _sourceColors.Length);
			return _sourceColors[randomColorIndex];
		}
	}
}