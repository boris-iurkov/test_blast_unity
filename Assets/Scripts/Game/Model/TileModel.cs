namespace Game.Model
{
	public class TileModel
	{
		public TileColor Color { get; private set; }
		public int X { get; private set; }
		public int Y { get; private set; }

		public void SetColor(TileColor color)
		{
			Color = color;
		}

		public void SetPositions(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}