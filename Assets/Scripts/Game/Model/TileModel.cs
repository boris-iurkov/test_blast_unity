namespace Game.Model
{
	public class TileModel
	{
		public TileColor Color { get; private set; }

		public void SetColor(TileColor color)
		{
			Color = color;
		}
	}
}