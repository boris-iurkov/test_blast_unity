using UnityEngine;

namespace Game.Model.Config
{
	[CreateAssetMenu(menuName = "Game/Game Config")]
	public class GameConfig : ScriptableObject
	{
		public int RowsCount;
		public int ColumnsCount;
		public int TileWidth;
		public int TileHeight;
		public int TargetScore;
		public int MaxMoves;
	}
}