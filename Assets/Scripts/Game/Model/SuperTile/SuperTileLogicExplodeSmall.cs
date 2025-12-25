using Game.Model.Data;
using UnityEngine;

namespace Game.Model.SuperTile
{
	public class SuperTileLogicExplodeSmall : ISuperTileLogic
	{
		public TileColor TileColor => TileColor.BombSmall;
		
		public void Activate(TileModel[,] tiles, Vector2Int position)
		{
			
		}
	}
}