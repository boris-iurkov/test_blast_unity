using System;

namespace Game.Model
{
	public class ScoreCounter
	{
		public int TargetScore { get; private set; }
		
		public event Action<int, int> OnScoreChanged;

		private int _score;
		
		public void Init(int targetScore)
		{
			_score = 0;
			TargetScore = targetScore;
			
			FireScoreChanged();
		}

		public void AddScoreForGroup(int groupSize)
		{
			int scorePerTile = GetScorePerTile(groupSize);
			int totalScore = scorePerTile * groupSize;
			_score += totalScore;
			
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
			OnScoreChanged?.Invoke(_score, TargetScore);
		}
	}
}