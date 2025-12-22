using UnityEngine;

namespace Game.Model.Config
{
	[CreateAssetMenu(menuName = "Game/Game Config")]
	public class GameConfig : ScriptableObject
	{
		public int rowsCount = 9;
		public int columnsCount = 9;
		public int targetScore = 500;
		public int maxMoves = 20;
	}
}