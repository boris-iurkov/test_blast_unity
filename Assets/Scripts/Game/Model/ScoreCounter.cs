using System;

namespace Game.Model
{
	public class ScoreCounter
	{
		public int TargetScore { get; private set; }
		public int Score { get; private set; }
		
		public event Action OnScoreChanged;

		public void Init(int targetScore)
		{
			Score = 0;
			TargetScore = targetScore;
		}

		public void AddScoreForGroup(int groupSize)
		{
			int scorePerTile = GetScorePerTileGroup(groupSize);
			int totalScore = scorePerTile * groupSize;
			Score += totalScore;
			
			FireScoreChanged();
		}
		
		public void AddScoreForSuperTile(int groupSize)
		{
			int scorePerTile = GetScorePerSuperTile(groupSize);
			int totalScore = scorePerTile * groupSize;
			Score += totalScore;
			
			FireScoreChanged();
		}

		public void AddScoreForBomb(int groupSize)
		{
			int scorePerTile = GetScorePerTileBomb();
			int totalScore = scorePerTile * groupSize;
			Score += totalScore;
			
			FireScoreChanged();
		}
		
		private int GetScorePerTileGroup(int groupSize)
		{
			if (groupSize < 1)
				return 0;

			return 10 * (groupSize - 1);
		}
		
		private int GetScorePerSuperTile(int groupSize)
		{
			return 10 * groupSize;
		}
		
		private int GetScorePerTileBomb()
		{
			return 20;
		}

		private void FireScoreChanged()
		{
			OnScoreChanged?.Invoke();
		}
	}
}