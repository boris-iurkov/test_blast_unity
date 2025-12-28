using Game.Model.SuperTile;

namespace Game.Controller.Factory
{
	public interface ISuperTileFactory
	{
		ISuperTileLogic CreateRandomSuperTile();
	}
}