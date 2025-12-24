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
		public int maxShuffles = 3;
		public int boosterSwapStartCount = 8;
		public int boosterBombStartCount = 5;
		public int boosterBombRadius = 1;
	}
}