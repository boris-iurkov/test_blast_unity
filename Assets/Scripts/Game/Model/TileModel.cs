using Game.Model.Data;
using Game.Model.SuperTile;

namespace Game.Model
{
	public class TileModel
	{
		public TileColor Color { get; private set; }
		public int Row { get; private set; }
		public int Column { get; private set; }
		public ISuperTileLogic SuperLogic { get; private set; }

		public void SetColor(TileColor color)
		{
			Color = color;
		}

		public void SetPositions(int row, int column)
		{
			Row = row;
			Column = column;
		}

		public void SetSuperTileLogic(ISuperTileLogic superLogic)
		{
			SuperLogic = superLogic;
		}
		
		public void Reset()
		{
			SuperLogic = null;
		}
	}
}