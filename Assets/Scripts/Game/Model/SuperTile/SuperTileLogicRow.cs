using Game.Model.Data;
using UnityEngine;

namespace Game.Model.SuperTile
{
	public class SuperTileLogicRow : ISuperTileLogic
	{
		public TileColor TileColor => TileColor.RocketsHorizontal;
		
		public void Activate(TileModel[,] tiles, Vector2Int position)
		{
			
		}
	}
}