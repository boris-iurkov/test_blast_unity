using UnityEngine;

namespace Game.Model.Config
{
	[CreateAssetMenu(menuName = "Game/Game Config")]
	public class GameConfig : ScriptableObject
	{
		public int rowsCount;
		public int columnsCount;
		public int tileWidth;
		public int tileHeight;
		public int gameFieldWidth;
		public int gameFieldHeight;
		public int targetScore;
		public int maxMoves;
	}
}