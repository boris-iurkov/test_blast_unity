using System;
using Game.Model;

namespace Game.Controller.Controllers
{
	public class BoosterSwapController
	{
		public TileModel FirstSelected { get; private set; }
		public TileModel SecondSelected { get; private set; }
		public bool IsSwapping { get; private set; }

		public event Action<TileModel> OnTileSelected;
		public event Action<TileModel> OnTileUnselected;

		public void OnTileClicked(TileModel tile)
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