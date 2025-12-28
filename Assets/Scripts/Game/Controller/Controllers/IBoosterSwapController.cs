using System;
using Game.Model;

namespace Game.Controller.Controllers
{
	public interface IBoosterSwapController
	{
		ITileData FirstSelected { get; }
		ITileData SecondSelected { get; }
		bool IsSwapping { get; }
		
		event Action<ITileData> OnTileSelected;
		event Action<ITileData> OnTileUnselected;
		
		void OnTileClicked(ITileData tile);
		void Reset();
	}
}