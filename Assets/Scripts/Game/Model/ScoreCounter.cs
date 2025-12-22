using System;

namespace Game.Model
{
	public class ScoreCounter
	{
		public int TargetScore { get; private set; }
		public int Score { get; private set; }
		
		public event Action<int, int> OnScoreChanged;

		public void Init(int targetScore)
		{
			Score = 0;
			TargetScore = targetScore;
			
			FireScoreChanged();
		}

		public void AddScoreForGroup(int groupSize)
		{
			int scorePerTile = GetScorePerTile(groupSize);
			int totalScore = scorePerTile * groupSize;
			Score += totalScore;
			
			FireScoreChanged();
		}
		
		private int GetScorePerTile(int groupSize)
		{
			if (groupSize < 1)
				return 0;

			return 10 * (groupSize - 1);
		}

		private void FireScoreChanged()
		{
			OnScoreChanged?.Invoke(Score, TargetScore);
		}
	}
}