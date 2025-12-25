using Game.Model.Data;
using UnityEngine;

namespace Game.Model.SuperTile
{
	public interface ISuperTileLogic
	{
		TileColor TileColor { get; }
		void Activate(TileModel[,] tiles, Vector2Int position);
	}
}