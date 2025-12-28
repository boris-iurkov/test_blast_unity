using System;
using Game.Model;

namespace Game.Controller.Controllers
{
	public class BoosterSwapController : IBoosterSwapController
	{
		public ITileData FirstSelected { get; private set; }
		public ITileData SecondSelected { get; private set; }
		public bool IsSwapping { get; private set; }

		public event Action<ITileData> OnTileSelected;
		public event Action<ITileData> OnTileUnselected;

		public void OnTileClicked(ITileData tile)
		{
			if (FirstSelected == null)
			{
				FirstSelected = tile;
				OnTileSelected?.Invoke(tile);
				return;
			}

			if (FirstSelected == tile)
			{
				OnTileUnselected?.Invoke(tile);
				FirstSelected = null;
				return;
			}

			SecondSelected = tile;
			IsSwapping = true;
			OnTileSelected?.Invoke(tile);
		}

		public void Reset()
		{
			FirstSelected = null;
			SecondSelected = null;
			IsSwapping = false;
		}
	}
}