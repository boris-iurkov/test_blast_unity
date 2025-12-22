using Game.Controller;
using Game.Model.Config;
using Game.Model.Data;
using Game.View;
using Game.View.Config;
using Game.View.Data;
using UnityEngine;

namespace Game.Core
{
	public class GameEntry : MonoBehaviour
	{
		[SerializeField] private GameConfig gameConfig;
		[SerializeField] private FieldConfig fieldConfig;
		
		[Space]
		[SerializeField] private GameFieldView gameFieldView;
		
		[Space]
		[SerializeField] private TileViewLibrary tileViewLibrary;
		[SerializeField] private TileViewPool tileViewPool;

		private GameController _gameController;

		private void Awake()
		{
			var gameConfigData = new GameConfigData
			{
				RowsCount = gameConfig.rowsCount,
				ColumnsCount = gameConfig.columnsCount,
				TargetScore = gameConfig.targetScore,
				MaxMoves = gameConfig.maxMoves
			};
			
			var fieldConfigData = new FieldConfigData
			{
				TileWidth = fieldConfig.tileWidth,
				TileHeight = fieldConfig.tileHeight,
				GameFieldWidth = fieldConfig.gameFieldWidth,
				GameFieldHeight = fieldConfig.gameFieldHeight
			};
			
			_gameController = new GameController();
			_gameController.Init(
				gameFieldView, 
				tileViewLibrary, 
				tileViewPool, 
				gameConfigData,
				fieldConfigData);
		}
	}
}