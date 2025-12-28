using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Model;
using Game.Model.Data;
using Game.View.Config;
using Game.View.Data;
using Game.View.Tile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.View
{
	public class GameFieldView : MonoBehaviour, IGameFieldView
	{
		[SerializeField] private RectTransform tilesParent;
		
		[Space]
		[SerializeField] private RemoveTileAnimationConfig removeTileAnimationConfig;
		[SerializeField] private FallTileAnimationConfig fallTileAnimationConfig;
		[SerializeField] private ShuffleAnimationConfig shuffleAnimationConfig;

		public event Action<int, int> OnTileClickRequested;
		public event Action FallCompleted;
		public event Action ShuffleCompleted;
		public event Action SwapTilesCompleted;
		
		private TileViewLibrary _tileViewLibrary;
		private TileView[,] _tiles;
		private TileViewPool _tileViewPool;

		private int _tileWidth;
		private int _tileHeight;
		private int _gameFieldWidth;
		private int _gameFieldHeight;
		private int _rowsCount;
		private int _columnsCount;
		
		private int[] _spawnOffsetsPerColumn;
		
		private readonly HashSet<Vector2Int> _fallingTiles = new();
		private int _currentFallPackCount;

		public void Init(
			TileViewLibrary tileViewLibrary,
			TileViewPool tileViewPool,
			FieldConfigData configData,
			int rowsCount,
			int columnsCount)
		{
			_tileViewLibrary = tileViewLibrary;
			_tileViewPool = tileViewPool;
			_tileWidth = configData.TileWidth;
			_tileHeight = configData.TileHeight;
			_gameFieldWidth = configData.GameFieldWidth;
			_gameFieldHeight = configData.GameFieldHeight;
			_rowsCount = rowsCount;
			_columnsCount = columnsCount;
			
			_tiles = new TileView[_rowsCount, _columnsCount];
			_spawnOffsetsPerColumn = new int[_columnsCount];
		}
		
		public void FillTile(ITileData tile)
		{
			TileView tileView = CreateTile(tile.Row, tile.Column, tile.Color);
			tileView.RectTransform.anchoredPosition = CalculateTilePosition(tile.Row, tile.Column);
		}

		public void ClearAllTiles()
		{
			for (var row = 0; row < _rowsCount; row++)
			for (var column = 0; column < _columnsCount; column++)
			{
				if (_tiles[row, column] != null)
				{
					TileView tile = _tiles[row, column];
					tile.Clicked -= OnTileClicked;
					tile.RectTransform.DOKill();
					_tileViewPool.ReturnTile(tile);
					_tiles[row, column] = null;
				}
			}
			
			if (tilesParent != null)
				tilesParent.DOKill();
			
			_fallingTiles.Clear();
			_currentFallPackCount = 0;
			
			for (var i = 0; i < _spawnOffsetsPerColumn.Length; i++)
				_spawnOffsetsPerColumn[i] = 0;
		}

		public void RemoveTileGroup(List<Vector2Int> group, int centerRow, int centerColumn)
		{
			const float stepDelay = 0.05f;

			foreach (Vector2Int pos in group)
			{
				TileView tile = _tiles[pos.x, pos.y];
				if (tile == null)
					continue;
				
				_tiles[pos.x, pos.y] = null;
				
				bool wasFalling = _fallingTiles.Contains(pos);
				if (wasFalling)
				{
					_fallingTiles.Remove(pos);
					_currentFallPackCount--;
					tile.RectTransform.DOKill();
				}

				int distance = Mathf.Abs(pos.x - centerRow) + Mathf.Abs(pos.y - centerColumn);
				float delay = distance * stepDelay;

				RemoveTile(tile, delay);
			}
		}
		
		public float GetDestroyGroupDuration(List<Vector2Int> group, Vector2Int origin, float stepDelay)
		{
			var maxDistance = 0;
			foreach (Vector2Int pos in group)
			{
				int d = Mathf.Abs(pos.x - origin.x) + Mathf.Abs(pos.y - origin.y);
				if (d > maxDistance)
					maxDistance = d;
			}

			return maxDistance * stepDelay;
		}

		public void FallTiles(List<TileFallData> fallTiles)
		{
			if (fallTiles.Count == 0)
			{
				if (_currentFallPackCount == 0 && _fallingTiles.Count == 0)
					FallCompleted?.Invoke();
				return;
			}
			
			int maxRow = _rowsCount - 1;
			
			_currentFallPackCount += fallTiles.Count;

			foreach (TileFallData fallTile in fallTiles)
			{
				TileView tile;

				bool isNewTile = fallTile.From.x >= _rowsCount;

				if (isNewTile)
				{
					tile = CreateTile(fallTile.Tile.Row, fallTile.Tile.Column, fallTile.Tile.Color);
					int additionalRow = _spawnOffsetsPerColumn[fallTile.Tile.Column];
					tile.RectTransform.anchoredPosition = CalculateTilePosition(_rowsCount + additionalRow + 1, fallTile.From.y);
					_spawnOffsetsPerColumn[fallTile.Tile.Column]++;
				}
				else
				{
					tile = _tiles[fallTile.From.x, fallTile.From.y];
					_tiles[fallTile.From.x, fallTile.From.y] = null;
				}

				Vector2 to = CalculateTilePosition(fallTile.To.x, fallTile.To.y);

				float distance = Vector2.Distance(tile.RectTransform.localPosition, to);
				float duration = distance / fallTileAnimationConfig.speed;

				float delay = fallTileAnimationConfig.startDelay +
				              (maxRow > 0 ? (float)fallTile.To.x / maxRow * fallTileAnimationConfig.cascadeDelayRange : 0f);

				_tiles[fallTile.To.x, fallTile.To.y] = tile;

				tile.SetPositions(fallTile.To.x, fallTile.To.y);

				_fallingTiles.Add(fallTile.To);

				tile.RectTransform
					.DOLocalMove(new Vector3(to.x, to.y), duration)
					.SetDelay(delay)
					.SetEase(fallTileAnimationConfig.ease)
					.OnComplete(() =>
					{
						_fallingTiles.Remove(fallTile.To);

						if (isNewTile)
							_spawnOffsetsPerColumn[fallTile.Tile.Column]--;
						
						_currentFallPackCount--;
						
						if (_currentFallPackCount == 0 && _fallingTiles.Count == 0)
						{
							_currentFallPackCount = 0;
							FallCompleted?.Invoke();
						}
					});
			}
		}
		
		public void ShuffleTiles()
		{
			var tilesList = new List<TileView>();
			foreach (TileView tileView in _tiles)
				tilesList.Add(tileView);

			for (int i = tilesList.Count - 1; i > 0; i--)
			{
				int j = Random.Range(0, i + 1);
				(tilesList[i], tilesList[j]) = (tilesList[j], tilesList[i]);
			}
			
			var index = 0;
			var delay = 0f;
			Sequence shuffleSequence = DOTween.Sequence();
			for (var x = 0; x < _rowsCount; x++)
			{
				for (var y = 0; y < _columnsCount; y++)
				{
					TileView view = tilesList[index++];
					Vector2 targetPos = CalculateTilePosition(x, y);
					view.SetPositions(x, y);

					float distance = Vector2.Distance(view.RectTransform.localPosition, targetPos);
					float duration = distance / shuffleAnimationConfig.speed;

					shuffleSequence.Insert(delay, view.RectTransform.DOLocalMove(targetPos, duration).SetEase(shuffleAnimationConfig.moveEase));
					delay += shuffleAnimationConfig.stepDelay;
				}
			}
			
			shuffleSequence.OnComplete(() =>
			{
				index = 0;
				for (var row = 0; row < _rowsCount; row++)
				for (var column = 0; column < _columnsCount; column++)
				{
					_tiles[row, column] = tilesList[index++];
					_tiles[row, column].RectTransform.SetSiblingIndex(row * _columnsCount + column);
				}

				ShuffleCompleted?.Invoke();
			});
		}

		public void UpdateTileLayers(ITileData tile1, ITileData tile2)
		{
			int index1 = _tiles[tile1.Row, tile1.Column].transform.GetSiblingIndex();
			int index2 = _tiles[tile2.Row, tile2.Column].transform.GetSiblingIndex();
			_tiles[tile1.Row, tile1.Column].RectTransform.SetSiblingIndex(index2);
			_tiles[tile2.Row, tile2.Column].RectTransform.SetSiblingIndex(index1);
		}

		public void UpdateTileView(int row, int column, TileColor tileColor)
		{
			Sprite sprite = _tileViewLibrary.GetSprite(tileColor);
			_tiles[row, column].SetSprite(sprite, tileColor);
		}

		public TileColor[,] GetCurrentTileColors()
		{
			var colors = new TileColor[_rowsCount, _columnsCount];
			for (var row = 0; row < _rowsCount; row++)
			for (var column = 0; column < _columnsCount; column++)
				colors[row, column] = _tiles[row, column].Color;
			return colors;
		}
		
		public bool IsTileFalling(int row, int column)
		{
			return _fallingTiles.Contains(new Vector2Int(row, column));
		}

		public bool HasFallingTiles()
		{
			return _fallingTiles.Count > 0;
		}

		public void SelectTile(ITileData tile)
		{
			TileView tileView = _tiles[tile.Row, tile.Column];
			tileView.RectTransform.DOKill();
			tileView.RectTransform.DOScale(new Vector3(0.75f, 0.75f, 1f), 0.3f);
		}

		public void UnselectTile(ITileData tile)
		{
			TileView tileView = _tiles[tile.Row, tile.Column];
			tileView.RectTransform.DOKill();
			tileView.RectTransform.DOScale(Vector3.one, 0.3f);
		}
		
		public void SwapTiles(ITileData tile1, ITileData tile2)
		{
			int row1 = tile1.Row;
			int column1 = tile1.Column;
			int row2 = tile2.Row;
			int column2 = tile2.Column;

			TileView firstTile = _tiles[row1, column1];
			TileView secondTile = _tiles[row2, column2];

			(_tiles[row1, column1], _tiles[row2, column2]) = (_tiles[row2, column2], _tiles[row1, column1]);
			
			firstTile.SetPositions(row2, column2);
			secondTile.SetPositions(row1, column1);
			
			Vector2 targetFirstTile = CalculateTilePosition(row2, column2);
			Vector2 targetSecondTile = CalculateTilePosition(row1, column1);

			float distance = Vector2.Distance(targetFirstTile, targetSecondTile);
			float duration = distance / shuffleAnimationConfig.speed;

			Sequence swapSequence = DOTween.Sequence();
			swapSequence.Insert(0, _tiles[row1, column1].RectTransform.DOLocalMove(targetSecondTile, duration)
					.SetEase(shuffleAnimationConfig.moveEase));
			swapSequence.Insert(0, _tiles[row2, column2].RectTransform.DOLocalMove(targetFirstTile, duration)
					.SetEase(shuffleAnimationConfig.moveEase));
			swapSequence.OnComplete(() =>
			{
				SwapTilesCompleted?.Invoke();
			});
		}

		private void RemoveTile(TileView tile, float delay)
		{
			Transform tileTransform = tile.transform;
			tileTransform.DOKill();

			Sequence sequence = DOTween.Sequence();
			sequence.SetDelay(delay);

			sequence.Append(
				tileTransform
					.DOScale(removeTileAnimationConfig.scaleUp, removeTileAnimationConfig.scaleUpDuration)
					.SetEase(removeTileAnimationConfig.scaleUpEase)
			);

			sequence.Append(
				tileTransform
					.DOScale(0f, removeTileAnimationConfig.scaleDownDuration)
					.SetEase(removeTileAnimationConfig.scaleDownEase)
			);

			sequence.OnComplete(() =>
			{
				tile.Clicked -= OnTileClicked;
				_tileViewPool.ReturnTile(tile);
			});
		}

		private TileView CreateTile(int row, int column, TileColor color)
		{
			Sprite sprite = _tileViewLibrary.GetSprite(color);

			TileView tileView = _tileViewPool.GetTile();
			tileView.transform.SetParent(tilesParent, false);

			tileView.SetSprite(sprite, color);
			tileView.SetPositions(row, column);

			tileView.Clicked += OnTileClicked;
			
			_tiles[row, column] = tileView;

			return tileView;
		}

		private Vector2 CalculateTilePosition(int row, int column)
		{
			return new Vector2(
				_tileWidth * column - _gameFieldWidth / 2 + _tileWidth / 2, 
				_tileHeight * row - _gameFieldHeight / 2 + _tileHeight / 2);
		}

		private void OnTileClicked(int row, int column)
		{
			if (IsTileFalling(row, column))
				return;
				
			OnTileClickRequested?.Invoke(row, column);
		}
	}
}