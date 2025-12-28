using Game.Controller.Data;
using Game.Model;
using Game.Model.Booster;
using Game.Model.Data;
using Game.View;

namespace Game.Controller.Controllers
{
	public class BoosterController : IBoosterController
	{
		public InteractionMode InteractionMode { get; private set; } = InteractionMode.Common;
		public IBoosterSwapController BoosterSwapController { get; private set; }
		public IBoosterBombCounter BoosterBomb { get; private set; }

		private IBoosterCounter _boosterSwap;
		private BoosterPanelView _boosterSwapView;
		private BoosterPanelView _boosterBombView;
		private IGameFieldView _gameFieldView;
		private IGameField _gameField;
		private IViewController _viewController;

		public void Init(
			IBoosterCounter boosterSwap,
			IBoosterBombCounter boosterBomb,
			BoosterPanelView boosterSwapView,
			BoosterPanelView boosterBombView,
			IGameFieldView gameFieldView,
			IGameField gameField,
			IViewController viewController,
			IBoosterSwapController boosterSwapController)
		{
			_boosterSwap = boosterSwap;
			BoosterBomb = boosterBomb;
			_boosterSwapView = boosterSwapView;
			_boosterBombView = boosterBombView;
			_gameFieldView = gameFieldView;
			_gameField = gameField;
			_viewController = viewController;

			_boosterSwapView.Init();
			_boosterSwapView.OnBoosterClicked += HandleBoosterSwapClicked;
			
			_boosterBombView.Init();
			_boosterBombView.OnBoosterClicked += HandleBoosterBombClicked;

			_boosterSwap.OnBoosterUsed += HandleBoosterSwapUsed;
			BoosterBomb.OnBoosterUsed += HandleBoosterBombUsed;

			BoosterSwapController = boosterSwapController;
			BoosterSwapController.OnTileSelected += HandleTileSelected;
			BoosterSwapController.OnTileUnselected += HandleTileUnselected;
		}

		public void UpdateViews()
		{
			_viewController.UpdateBoosterSwapView();
			_viewController.UpdateBoosterBombView();
		}
		
		public void HandleSwapTilesCompleted()
		{
			_gameFieldView.UpdateTileLayers(BoosterSwapController.FirstSelected, BoosterSwapController.SecondSelected);
			
			BoosterSwapController.Reset();

			TileColor[,] colors = _gameFieldView.GetCurrentTileColors();
			_gameField.SetColors(colors);
			
			BoosterSwapController.Reset();
			InteractionMode = InteractionMode.Common;
			UpdateSelectionsByInteractionMode();
		}

		public void Reset()
		{
			InteractionMode = InteractionMode.Common;
			BoosterSwapController.Reset();
			_viewController.UnselectAllBoosters();
		}

		private void HandleBoosterSwapClicked()
		{
			if (_boosterSwap.Count <= 0)
				return;
			
			InteractionMode = InteractionMode != InteractionMode.BoosterSwap
				? InteractionMode.BoosterSwap
				: InteractionMode.Common;

			UpdateSelectionsByInteractionMode();
		}
		
		private void HandleBoosterSwapUsed()
		{
			_viewController.UpdateBoosterSwapView();
		}

		private void HandleBoosterBombClicked()
		{
			if (BoosterBomb.Count <= 0)
				return;
			
			InteractionMode = InteractionMode != InteractionMode.BoosterBomb
				? InteractionMode.BoosterBomb
				: InteractionMode.Common;
			
			UpdateSelectionsByInteractionMode();
		}
		
		private void HandleBoosterBombUsed()
		{
			_viewController.UpdateBoosterBombView();
		}

		private void UpdateSelectionsByInteractionMode()
		{
			switch (InteractionMode)
			{
				case InteractionMode.BoosterSwap:
					_viewController.UpdateBoosterSwapSelection(true);
					_viewController.UpdateBoosterBombSelection(false);
					break;
				
				case InteractionMode.BoosterBomb:
					_viewController.UpdateBoosterSwapSelection(false);
					_viewController.UpdateBoosterBombSelection(true);
					break;
				
				default:
					_viewController.UpdateBoosterSwapSelection(false);
					_viewController.UpdateBoosterBombSelection(false);
					break;
			}
			
			if (InteractionMode != InteractionMode.BoosterSwap)
			{
				TileModel firstTile = BoosterSwapController.FirstSelected;
				if (firstTile != null)
					_gameFieldView.UnselectTile(firstTile);
				BoosterSwapController.Reset();
			}
		}

		private void HandleTileSelected(TileModel tile)
		{
			_gameFieldView.SelectTile(tile);
			if (BoosterSwapController.SecondSelected != null)
			{
				TileModel firstTile = BoosterSwapController.FirstSelected;
				TileModel secondTile = BoosterSwapController.SecondSelected;

				_gameFieldView.UnselectTile(firstTile);
				_gameFieldView.UnselectTile(secondTile);
				
				_gameFieldView.SwapTiles(firstTile, secondTile);
				_gameField.SwapTiles(firstTile, secondTile);
				
				_boosterSwap.Use();
			}
		}
		
		private void HandleTileUnselected(TileModel tile)
		{
			_gameFieldView.UnselectTile(tile);
		}
	}
}