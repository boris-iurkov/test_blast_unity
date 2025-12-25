using System.Collections.Generic;
using Game.Model.Data;
using UnityEngine;

namespace Game.Model.SuperTile
{
	public class SuperTileLogicExplodeBig : ISuperTileLogic
	{
		public TileColor TileColor => TileColor.BombBig;
		
		public List<Vector2Int> GetAffectedTiles(TileModel[,] tiles, Vector2Int position)
		{
			int rowsCount = tiles.GetLength(0);
			int columnsCount = tiles.GetLength(1);
			
			var result = new List<Vector2Int>();
			for (var i = 0; i < rowsCount; i++)
			for (var j = 0; j < columnsCount; j++)
				result.Add(new Vector2Int(i, j));
			return result;
		}
	}
}