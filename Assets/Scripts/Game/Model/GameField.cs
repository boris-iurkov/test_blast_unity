using System;
using Random = UnityEngine.Random;

namespace Game.Model
{
	public class GameField
	{
		private TileColor[] _sourceColors;
		private TileModel[,] _tiles;

		public int RowsCount { get; private set; }
		public int ColumnsCount { get; private set; }

		public void Init(int rowsCount, int columnsCount)
		{
			RowsCount = rowsCount;
			ColumnsCount = columnsCount;
			
			InitSourceColors();
			InitStartTiles();
		}

		private void InitSourceColors()
		{
			_sourceColors = (TileColor[])Enum.GetValues(typeof(TileColor));
		}

		private void InitStartTiles()
		{
			_tiles = new TileModel[RowsCount, ColumnsCount];
			
			for (var row = 0; row < RowsCount; row++)
			{
				for (var column = 0; column < ColumnsCount; column++)
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