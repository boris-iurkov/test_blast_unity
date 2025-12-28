using System;

namespace Game.Model
{
	public interface IScoreCounter
	{
		int TargetScore { get; }
		int Score { get; }
		event Action OnScoreChanged;
		
		void Init(int targetScore);
		void AddScoreForGroup(int groupSize);
		void AddScoreForSuperTile(int groupSize);
		void AddScoreForBomb(int groupSize);
	}
}