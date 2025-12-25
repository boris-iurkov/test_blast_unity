using System.Collections.Generic;
using Game.Model.Data;
using UnityEngine;

namespace Game.Model.SuperTile
{
	public class SuperTileLogicColumn : ISuperTileLogic
	{
		public TileColor TileColor => TileColor.RocketsVertical;
		
		public List<Vector2Int> GetAffectedTiles(TileModel[,] tiles, Vector2Int position)
		{
			var result = new List<Vector2Int>();
			int rows = tiles.GetLength(0);
			for (var i = 0; i < rows; i++)
				result.Add(new Vector2Int(i, position.y));
			return result;
		}
	}
}