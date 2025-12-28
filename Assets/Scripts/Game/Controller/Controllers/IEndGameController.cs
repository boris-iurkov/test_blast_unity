using System;

namespace Game.Controller.Controllers
{
	public interface IEndGameController
	{
		bool IsEndGame { get; }
		event Action OnRestartRequested;
		
		void SetShufflesCount(int count);
		void TryEndGame();
		void ShowEndGamePopup();
		void Reset();
	}
}