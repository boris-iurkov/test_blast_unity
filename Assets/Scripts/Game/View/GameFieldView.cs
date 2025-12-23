using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Model;
using Game.Model.Data;
using Game.View.Config;
using Game.View.Data;
using Game.View.Tile;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.View
{
	public class GameFieldView : MonoBehaviour
	{
		[SerializeField] private RectTransform tilesParent;
		
		[Space]
		[SerializeField] private RemoveTileAnimationConfig removeTileAnimationConfig;
		[SerializeField] private FallTileAnimationConfig fallTileAnimationConfig;
		[SerializeField] private ShuffleAnimationConfig shuffleAnimationConfig;
		
		[Space]
		[SerializeField] private TextMeshProUGUI labelMoves;
		[SerializeField] private TextMeshProUGUI labelScore;

		public event Action<int, int> OnTileClickRequested;
		public event Action FallCompleted;
		public event Action ShuffleCompleted;
		
		private GameField _gameField;
		
		private TileViewLibrary _tileViewLibrary;
		private TileView[,] _tiles;
		private TileViewPool _tileViewPool;

		private int _tileWidth;
		private int _tileHeight;
		private int _gameFieldWidth;
		private int _gameFieldHeight;
		
		private int[] _spawnOffsetsPerColumn;
		
		private readonly HashSet<Vector2Int> _fallingTiles = new();
		private int _currentFallPackCount = 0;

		public void Init(
			GameField gameField, 
			TileViewLibrary tileViewLibrary,
			TileViewPool tileViewPool,
			FieldConfigData configData)
		{
			_gameField = gameField;
			_tileViewLibrary = tileViewLibrary;
			_tileViewPool = tileViewPool;
			_tileWidth = configData.TileWidth;
			_tileHeight = configData.TileHeight;
			_gameFieldWidth = configData.GameFieldWidth;
			_gameFieldHeight = configData.GameFieldHeight;
			
			_tiles = new TileView[_gameField.RowsCount, _gameField.ColumnsCount];
			_spawnOffsetsPerColumn = new int[_gameField.ColumnsCount];

			FillField();
		}

		public void RemoveTileGroup(List<Vector2Int> group)
		{
			foreach (Vector2Int positions in group)
			{
				TileView tile = _tiles[positions.x, positions.y];
				_tiles[positions.x, positions.y] = null;
				tile.SetClickable(false);
				RemoveTile(tile);
			}
		}

		public void FallTiles(List<TileFallData> fallTiles)
		{
			int maxRow = _gameField.RowsCount - 1;
			
			_currentFallPackCount += fallTiles.Count;

			foreach (TileFallData fallTile in fallTiles)
			{
				TileView tile;

				bool isNewTile = fallTile.From.x >= _gameField.RowsCount;

				if (isNewTile)
				{
					tile = CreateTile(fallTile.Tile);
					int additionalRow = _spawnOffsetsPerColumn[fallTile.Tile.Column];
					tile.RectTransform.anchoredPosition = CalculateTilePosition(_gameField.RowsCount + additionalRow + 1, fallTile.From.y);
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
				tile.SetClickable(false);

				_fallingTiles.Add(fallTile.To);

				tile.RectTransform
					.DOLocalMove(new Vector3(to.x, to.y), duration)
					.SetDelay(delay)
					.SetEase(fallTileAnimationConfig.ease)
					.OnComplete(() =>
					{
						_fallingTiles.Remove(fallTile.To);
						tile.SetClickable(true);

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
			int rows = _gameField.RowsCount;
			int columns = _gameField.ColumnsCount;
			
			var tilesList = new List<TileView>();
			foreach (TileView tileView in _tiles)
			{
				tilesList.Add(tileView);
				tileView.SetClickable(false);
			}

			for (int i = tilesList.Count - 1; i > 0; i--)
			{
				int j = Random.Range(0, i + 1);
				(tilesList[i], tilesList[j]) = (tilesList[j], tilesList[i]);
			}
			
			var index = 0;
			var delay = 0f;
			Sequence shuffleSequence = DOTween.Sequence();
			for (var x = 0; x < rows; x++)
			{
				for (var y = 0; y < columns; y++)
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
				for (var row = 0; row < rows; row++)
				for (var column = 0; column < columns; column++)
				{
					_tiles[row, column] = tilesList[index++];
					_tiles[row, column].RectTransform.SetSiblingIndex(row * columns + column);
				}

				foreach (TileView tileView in _tiles)
					tileView.SetClickable(true);
				
				ShuffleCompleted?.Invoke();
			});
		}

		public TileColor[,] GetCurrentTileColors()
		{
			var colors = new TileColor[_gameField.RowsCount, _gameField.ColumnsCount];
			for (var row = 0; row < _gameField.RowsCount; row++)
			for (var column = 0; column < _gameField.ColumnsCount; column++)
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

		public void UpdateMovesCount(int movesLeft)
		{
			labelMoves.SetText(movesLeft.ToString());
		}
		
		public void UpdateScoreCount(int score, int targetScore)
		{
			labelScore.SetText(score + "/" + targetScore);
		}

		private void RemoveTile(TileView tile)
		{
			Transform tileTransform = tile.transform;
			tileTransform.DOKill();

			Sequence sequence = DOTween.Sequence();
			sequence.Append(
				tileTransform.DOScale(removeTileAnimationConfig.scaleUp, removeTileAnimationConfig.scaleUpDuration)
					.SetEase(removeTileAnimationConfig.scaleUpEase)
				);
			sequence.Append(
				tileTransform.DOScale(0f, removeTileAnimationConfig.scaleDownDuration)
					.SetEase(removeTileAnimationConfig.scaleDownEase)
				);
			sequence.OnComplete(() =>
			{
				_tileViewPool.ReturnTile(tile);
			});
		}

		private void FillField()
		{
			int rows = _gameField.Tiles.GetLength(0);
			int columns = _gameField.Tiles.GetLength(1);
			
			for (var row = 0; row < rows; row++)
			{
				for (var column = 0; column < columns; column++)
				{
					TileModel tile = _gameField.Tiles[row, column];
					TileView tileView = CreateTile(tile);
					tileView.RectTransform.anchoredPosition = CalculateTilePosition(row, column);
				}
			}
		}

		private TileView CreateTile(TileModel tile)
		{
			Sprite sprite = _tileViewLibrary.GetSprite(tile.Color);
			int row = tile.Row;
			int column = tile.Column;
			
			TileView tileView = _tileViewPool.GetTile();
			tileView.transform.SetParent(tilesParent, false);

			tileView.SetSprite(sprite, tile.Color);
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