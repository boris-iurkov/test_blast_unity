using System;
using Game.Model;
using Random = UnityEngine.Random;

namespace Game.Core
{
	public class Board
	{
		private TileColor[] _sourceColors;
		private TileModel[,] _tiles;

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
			int rows = GameConfig.BoardRows;
			int columns = GameConfig.BoardColumns;
			
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