using System;
using Random = UnityEngine.Random;

namespace Game.Model
{
	public class GameField
	{
		private TileColor[] _sourceColors;
		private TileModel[,] _tiles;

		private int _rowsCount;
		private int _columnsCount;

		public int Width => _tiles.GetLength(0);
		public int Height => _tiles.GetLength(1);

		public void Init(int rowsCount, int columnsCount)
		{
			_rowsCount = rowsCount;
			_columnsCount = columnsCount;
			
			InitSourceColors();
			InitStartTiles();
		}

		private void InitSourceColors()
		{
			_sourceColors = (TileColor[])Enum.GetValues(typeof(TileColor));
		}

		private void InitStartTiles()
		{
			_tiles = new TileModel[_rowsCount, _columnsCount];
			
			for (var row = 0; row < _rowsCount; row++)
			{
				for (var column = 0; column < _columnsCount; column++)
				{
					var tile = new TileModel();
					tile.SetColor(GetRandomTileColor());

					_tiles[row, column] = tile;
				}
			}
		}

		private TileColor GetRandomTileColor()
		{
			int randomColorIndex = Random.Range(0, _sourceColors.Length);
			return _sourceColors[randomColorIndex];
		}
	}
}