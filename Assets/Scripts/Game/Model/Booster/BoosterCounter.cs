using System;

namespace Game.Model.Booster
{
	public class BoosterCounter : IBoosterCounter
	{
		public int StartCount { get; private set; }
		public int Count { get; private set; }

		public event Action OnBoosterUsed;

		public void Init(int startCount)
		{
			StartCount = startCount;
			Count = startCount;
		}

		public void Use()
		{
			if (Count <= 0)
				return;

			Count--;
			
			OnBoosterUsed?.Invoke();
		}
	}
}