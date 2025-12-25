using Game.Model.Data;
using UnityEngine;

namespace Game.Model.SuperTile
{
	public class SuperTileLogicColumn : ISuperTileLogic
	{
		public TileColor TileColor => TileColor.RocketsVertical;
		
		public void Activate(TileModel[,] tiles, Vector2Int position)
		{
			
		}
	}
}