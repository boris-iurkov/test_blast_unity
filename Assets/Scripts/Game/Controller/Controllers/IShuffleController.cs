namespace Game.Controller.Controllers
{
	public interface IShuffleController
	{
		bool IsShuffling { get; }
		
		void TryEndGameByShuffle();
		void Reset();
	}
}