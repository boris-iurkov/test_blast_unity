using Game.Controller;
using Game.Model.Config;
using Game.View;
using UnityEngine;

namespace Game.Core
{
	public class GameEntry : MonoBehaviour
	{
		[SerializeField] private GameConfig gameConfig;
		
		[Space]
		[SerializeField] private GameFieldView gameFieldView;
		
		[Space]
		[SerializeField] private TileViewLibrary tileViewLibrary;
		[SerializeField] private TileViewPool tileViewPool;

		private GameController _gameController;

		private void Awake()
		{
			_gameController = new GameController();
			_gameController.Init(
				gameFieldView, 
				tileViewLibrary, 
				tileViewPool, 
				gameConfig.RowsCount, 
				gameConfig.ColumnsCount,
				gameConfig.TileWidth,
				gameConfig.TileHeight);
		}
	}
}