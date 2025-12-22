using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Model;
using Game.Model.Data;
using Game.View.Config;
using UnityEngine;

namespace Game.View
{
	public class GameFieldView : MonoBehaviour
	{
		[SerializeField] private RectTransform tilesParent;
		[SerializeField] private RemoveTileAnimationConfig removeTileAnimationConfig;
		[SerializeField] private FallTileAnimationConfig fallTileAnimationConfig;

		public event Action<int, int> OnTileClickRequested;
		
		private GameField _gameField;
		private TileViewLibrary _tileViewLibrary;
		private TileView[,] _tiles;
		private TileViewPool _tileViewPool;

		private int _tileWidth;
		private int _tileHeight;
		private int _gameFieldWidth;
		private int _gameFieldHeight;
		
		private readonly HashSet<Vector2Int> _fallingTiles = new();

		public void Init(
			GameField gameField, 
			TileViewLibrary tileViewLibrary,
			TileViewPool tileViewPool,
			int tileWidth,
			int tileHeight,
			int gameFieldWidth,
			int gameFieldHeight)
		{
			_gameField = gameField;
			_tileViewLibrary = tileViewLibrary;
			_tileViewPool = tileViewPool;
			_tileWidth = tileWidth;
			_tileHeight = tileHeight;
			_gameFieldWidth = gameFieldWidth;
			_gameFieldHeight = gameFieldHeight;
			
			_tiles = new TileView[_gameField.RowsCount, _gameField.ColumnsCount];
			
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

		public void ApplyFallTiles(List<TileFallData> fallTiles)
		{
			int maxRow = _gameField.RowsCount - 1;
			
			foreach (TileFallData fallTile in fallTiles)
			{
				TileView tile = _tiles[fallTile.From.x, fallTile.From.y];
				Vector2 from = CalculateTilePosition(fallTile.From.x, fallTile.From.y);
				Vector2 to = CalculateTilePosition(fallTile.To.x, fallTile.To.y);
				
				float distance = Vector2.Distance(from, to);
				float duration = distance / fallTileAnimationConfig.speed;
				float delay = fallTileAnimationConfig.startDelay + (maxRow > 0 ? (float)fallTile.From.x / maxRow * fallTileAnimationConfig.cascadeDelayRange : 0f);
				
				_tiles[fallTile.From.x, fallTile.From.y] = null;
				_tiles[fallTile.To.x, fallTile.To.y] = tile;
				
				tile.SetPositions(fallTile.To.x, fallTile.To.y);
				
				_fallingTiles.Add(fallTile.To);
				
				tile.SetClickable(false);
				
				tile.RectTransform
					.DOLocalMove(new Vector3(to.x, to.y), duration)
					.SetDelay(delay)
					.SetEase(fallTileAnimationConfig.ease)
					.OnComplete(() =>
					{
						_fallingTiles.Remove(fallTile.To);
						tile.SetClickable(true);
					});
			}
		}
		
		public bool IsTileFalling(int row, int column)
		{
			return _fallingTiles.Contains(new Vector2Int(row, column));
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
					Sprite sprite = _tileViewLibrary.GetSprite(tile.Color);
					CreateTile(sprite, tile.Row, tile.Column);
				}
			}
		}

		private void CreateTile(Sprite sprite, int row, int column)
		{
			TileView tileView = _tileViewPool.GetTile();
			tileView.transform.SetParent(tilesParent, false);
			tileView.RectTransform.anchoredPosition = CalculateTilePosition(row, column);
			
			tileView.SetSprite(sprite);
			tileView.SetPositions(row, column);

			tileView.Clicked += OnTileClicked;
			
			_tiles[row, column] = tileView;
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