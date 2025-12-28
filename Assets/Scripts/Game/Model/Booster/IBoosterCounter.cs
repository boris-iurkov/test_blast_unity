using System;

namespace Game.Model.Booster
{
	public interface IBoosterCounter
	{
		int StartCount { get; }
		int Count { get; }
		event Action OnBoosterUsed;
		
		void Init(int startCount);
		void Use();
	}
}