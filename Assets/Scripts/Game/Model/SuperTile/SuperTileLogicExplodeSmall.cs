using System.Collections.Generic;
using Game.Model.Data;
using UnityEngine;

namespace Game.Model.SuperTile
{
	public class SuperTileLogicExplodeSmall : ISuperTileLogic
	{
		public TileColor TileColor => TileColor.BombSmall;

		private int _radius;

		public void Init(int radius)
		{
			_radius = radius;
		}
		
		public List<Vector2Int> GetAffectedTiles(TileModel[,] tiles, Vector2Int position)
		{
			int rowsCount = tiles.GetLength(0);
			int columnsCount = tiles.GetLength(1);
			
			var result = new List<Vector2Int>();
			for (int dx = -_radius; dx <= _radius; dx++)
			for (int dy = -_radius; dy <= _radius; dy++)
			{
				Vector2Int targetPosition = position + new Vector2Int(dx, dy);
				if (IsTileInsideField(targetPosition.x, targetPosition.y, rowsCount, columnsCount))
					result.Add(new Vector2Int(targetPosition.x, targetPosition.y));
			}
			return result;
		}
		
		private bool IsTileInsideField(int row, int column, int rowsCount, int columnsCount)
		{
			return row >= 0 &&
			       row < rowsCount &&
			       column >= 0 &&
			       column < columnsCount;
		}
	}
}