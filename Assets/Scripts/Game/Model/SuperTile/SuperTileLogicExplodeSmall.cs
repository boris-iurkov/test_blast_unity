using System.Collections.Generic;
using Game.Model.Data;
using UnityEngine;

namespace Game.Model.SuperTile
{
	public class SuperTileLogicExplodeSmall : ISuperTileLogic
	{
		public TileColor TileColor => TileColor.BombSmall;
		
		public List<Vector2Int> GetAffectedTiles(TileModel[,] tiles, Vector2Int position)
		{
			var result = new List<Vector2Int>();
			return result;
		}
	}
}