using Game.Model.Data;
using UnityEngine;

namespace Game.Model.SuperTile
{
	public class SuperTileLogicExplodeBig : ISuperTileLogic
	{
		public TileColor TileColor => TileColor.BombBig;
		
		public void Activate(TileModel[,] tiles, Vector2Int position)
		{
			
		}
	}
}