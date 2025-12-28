using Game.Controller.Data;
using Game.Model.Booster;

namespace Game.Controller.Controllers
{
	public interface IBoosterController
	{
		InteractionMode InteractionMode { get; }
		IBoosterSwapController BoosterSwapController { get; }
		IBoosterBombCounter BoosterBomb { get; }
		
		void UpdateViews();
		void HandleSwapTilesCompleted();
		void Reset();
	}
}