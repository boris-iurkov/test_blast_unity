using System.Collections.Generic;
using Game.Model.Data;
using UnityEngine;

namespace Game.Model.SuperTile
{
	public class SuperTileLogicRow : ISuperTileLogic
	{
		public TileColor TileColor => TileColor.RocketsHorizontal;
		
		public List<Vector2Int> GetAffectedTiles(TileModel[,] tiles, Vector2Int position)
		{
			var result = new List<Vector2Int>();
			int columns = tiles.GetLength(1);
			for (var i = 0; i < columns; i++)
				result.Add(new Vector2Int(position.x, i));
			return result;
		}
	}
}