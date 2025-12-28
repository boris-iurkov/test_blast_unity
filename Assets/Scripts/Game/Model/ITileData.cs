using Game.Model.Data;

namespace Game.Model
{
	public interface ITileData
	{
		int Row { get; }
		int Column { get; }
		TileColor Color { get; }
	}
}