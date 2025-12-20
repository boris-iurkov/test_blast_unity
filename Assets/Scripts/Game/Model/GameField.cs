using System;
using Random = UnityEngine.Random;

namespace Game.Model
{
	public class GameField
	{
		private TileColor[] _sourceColors;
		private TileModel[,] _tiles;

		public int Width => _tiles.GetLength(0);
		public int Height => _tiles.GetLength(1);

		public void Init()
		{
			InitSourceColors();
			InitStartTiles();
		}

		private void InitSourceColors()
		{
			_sourceColors = (TileColor[])Enum.GetValues(typeof(TileColor));
		}

		private void InitStartTiles()
		{
			int rows = GameConfig.GameFieldRowsCount;
			int columns = GameConfig.GameFieldColumnsCount;
			
			_tiles = new TileModel[rows, columns];
			
			for (var row = 0; row < rows; row++)
			{
				for (var column = 0; column < columns; column++)
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