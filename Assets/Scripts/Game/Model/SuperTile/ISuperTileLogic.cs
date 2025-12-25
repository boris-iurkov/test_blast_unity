using System.Collections.Generic;
using Game.Model.Data;
using UnityEngine;

namespace Game.Model.SuperTile
{
	public interface ISuperTileLogic
	{
		TileColor TileColor { get; }
		List<Vector2Int> GetAffectedTiles(TileModel[,] tiles, Vector2Int position);
	}
}