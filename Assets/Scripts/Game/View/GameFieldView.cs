using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Model;
using Game.View.Animation;
using UnityEngine;

namespace Game.View
{
	public class GameFieldView : MonoBehaviour
	{
		[SerializeField] private RectTransform tilesParent;
		[SerializeField] private RemoveTileAnimationSettings removeAnimation;

		public event Action<int, int> OnTileClickRequested;
		
		private GameField _gameField;
		private TileViewLibrary _tileViewLibrary;
		private TileView[,] _tiles;
		private TileViewPool _tileViewPool;

		private int _tileWidth;
		private int _tileHeight;
		private int _gameFieldWidth;
		private int _gameFieldHeight;

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
		
		private void RemoveTile(TileView tile)
		{
			Transform tileTransform = tile.transform;
			tileTransform.DOKill();

			Sequence sequence = DOTween.Sequence();
			sequence.Append(
				tileTransform.DOScale(removeAnimation.scaleUp, removeAnimation.scaleUpDuration)
					.SetEase(removeAnimation.scaleUpEase)
			);
			sequence.Append(
				tileTransform.DOScale(0f, removeAnimation.scaleDownDuration)
					.SetEase(removeAnimation.scaleDownEase)
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
			OnTileClickRequested?.Invoke(row, column);
		}
	}
}