using System.Collections.Generic;
using DG.Tweening;
using Game.Controller.Data;
using Game.Controller.Factory;
using Game.Model;
using Game.Model.Data;
using Game.Model.SuperTile;
using Game.View;
using UnityEngine;

namespace Game.Controller.Controllers
{
	public class TileInteractionController
	{
		private IGameField _gameField;
		private IGameFieldView _gameFieldView;
		private IScoreCounter _scoreCounter;
		private IMovesCounter _movesCounter;
		private BoosterController _boosterController;
		private SuperTileFactory _superTileFactory;

		public void Init(
			IGameField gameField,
			IGameFieldView gameFieldView,
			IScoreCounter scoreCounter,
			IMovesCounter movesCounter,
			BoosterController boosterController,
			SuperTileFactory superTileFactory)
		{
			_gameField = gameField;
			_gameFieldView = gameFieldView;
			_scoreCounter = scoreCounter;
			_movesCounter = movesCounter;
			_boosterController = boosterController;
			_superTileFactory = superTileFactory;
		}

		public void HandleTileClick(int row, int column, bool isEndGame, bool isShuffling)
		{
			if (!CanProcessTileClick(row, column, isEndGame, isShuffling))
				return;

			TileModel clickedTile = _gameField.GetTile(row, column);
			if (clickedTile == null)
				return;

			if (TryHandleBoosterSwap(row, column))
				return;

			bool isSuperTile = IsSuperTile(row, column);
			List<Vector2Int> group = GetTileGroup(row, column, isSuperTile);
			group = FilterFallingTiles(group);
			
			if (!TryProcessCommonMode(row, column, group, isSuperTile, out int groupCount, out bool superTileCreated))
				return;
			
			List<Vector2Int> finalGroup = ActivateSuperTilesInGroup(group);
			ProcessGroupRemoval(finalGroup, row, column, superTileCreated);
			AddScoreAndMakeMove(groupCount, isSuperTile);
			
			UseBoosterIfNeeded();
			_boosterController.Reset();
		}

		private bool CanProcessTileClick(int row, int column, bool isEndGame, bool isShuffling)
		{
			return !isEndGame
			       && !isShuffling
			       && !_boosterController.BoosterSwapController.IsSwapping
			       && !_gameFieldView.IsTileFalling(row, column);
		}

		private bool TryHandleBoosterSwap(int row, int column)
		{
			if (_boosterController.InteractionMode != InteractionMode.BoosterSwap)
				return false;

			TileModel tile = _gameField.GetTile(row, column);
			if (tile != null)
				_boosterController.BoosterSwapController.OnTileClicked(tile);
			return true;
		}

		private bool IsSuperTile(int row, int column)
		{
			return _gameField.IsSuperTile(row, column);
		}

		private List<Vector2Int> GetTileGroup(int row, int column, bool isSuperTile)
		{
			if (_boosterController.InteractionMode == InteractionMode.BoosterBomb)
				return _gameField.GetBoosterBombTileGroup(row, column, _boosterController.BoosterBomb.Radius);

			return isSuperTile
				? _gameField.GetSuperTileGroup(row, column)
				: _gameField.GetCommonTileGroup(row, column);
		}

		private List<Vector2Int> FilterFallingTiles(List<Vector2Int> group)
		{
			var filteredGroup = new List<Vector2Int>();
			foreach (Vector2Int pos in group)
			{
				if (!_gameFieldView.IsTileFalling(pos.x, pos.y))
					filteredGroup.Add(pos);
			}
			return filteredGroup;
		}

		private bool TryProcessCommonMode(int row, int column, List<Vector2Int> group, bool isSuperTile, out int groupCount, out bool superTileCreated)
		{
			groupCount = group.Count;
			superTileCreated = false;

			if (_boosterController.InteractionMode != InteractionMode.Common)
				return true;

			if (groupCount < 2)
				return false;

			if (groupCount >= _gameField.MinSuperTileGroupSize && !isSuperTile)
			{
				CreateSuperTile(row, column, group);
				superTileCreated = true;
			}

			return true;
		}

		private void CreateSuperTile(int row, int column, List<Vector2Int> group)
		{
			group.RemoveAll(tile => tile.x == row && tile.y == column);
			ISuperTileLogic superTileLogic = _superTileFactory.CreateRandomSuperTile();
			_gameField.SetSuperTileLogic(row, column, superTileLogic);
			
			TileModel tile = _gameField.GetTile(row, column);
			if (tile != null)
				_gameFieldView.UpdateTileView(row, column, tile.Color);
		}

		private void ProcessGroupRemoval(List<Vector2Int> finalGroup, int row, int column, bool excludeSuperTilePosition)
		{
			if (excludeSuperTilePosition)
				finalGroup.Remove(new Vector2Int(row, column));

			_gameField.RemoveTileGroup(finalGroup);
			_gameFieldView.RemoveTileGroup(finalGroup, row, column);

			float destroyDuration = _gameFieldView.GetDestroyGroupDuration(finalGroup, new Vector2Int(row, column), 0.05f);
			DOVirtual.DelayedCall(destroyDuration, () =>
			{
				List<TileFallData> fallingTiles = _gameField.ApplyFallTiles();
				_gameFieldView.FallTiles(fallingTiles);
			});
		}

		private void AddScoreAndMakeMove(int groupCount, bool isSuperTile)
		{
			if (_boosterController.InteractionMode == InteractionMode.BoosterBomb)
				_scoreCounter.AddScoreForBomb(groupCount);
			else if (isSuperTile)
				_scoreCounter.AddScoreForSuperTile(groupCount);
			else
				_scoreCounter.AddScoreForGroup(groupCount);

			_movesCounter.MakeMove();
		}

		private void UseBoosterIfNeeded()
		{
			if (_boosterController.InteractionMode == InteractionMode.BoosterBomb)
				_boosterController.BoosterBomb.Use();
		}

		private List<Vector2Int> ActivateSuperTilesInGroup(List<Vector2Int> group)
		{
			var finalGroup = new HashSet<Vector2Int>(group);
			
			var superTilesToActivate = new List<Vector2Int>();
			foreach (Vector2Int pos in group)
			{
				if (_gameField.IsSuperTile(pos.x, pos.y))
					superTilesToActivate.Add(pos);
			}
			
			foreach (Vector2Int superTilePos in superTilesToActivate)
			{
				List<Vector2Int> superTileGroup = _gameField.GetSuperTileGroup(superTilePos.x, superTilePos.y);
				foreach (Vector2Int affectedPos in superTileGroup)
					finalGroup.Add(affectedPos);
			}
			
			return new List<Vector2Int>(finalGroup);
		}
	}
}