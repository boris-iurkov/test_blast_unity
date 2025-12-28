using System;
using Game.Model;

namespace Game.Controller.Controllers
{
	public interface IBoosterSwapController
	{
		TileModel FirstSelected { get; }
		TileModel SecondSelected { get; }
		bool IsSwapping { get; }
		
		event Action<TileModel> OnTileSelected;
		event Action<TileModel> OnTileUnselected;
		
		void OnTileClicked(TileModel tile);
		void Reset();
	}
}