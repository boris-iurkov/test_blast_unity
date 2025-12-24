using Game.Controller;
using Game.Model.Config;
using Game.Model.Data;
using Game.View;
using Game.View.Config;
using Game.View.Data;
using Game.View.Popup;
using Game.View.Tile;
using UnityEngine;

namespace Game.Core
{
	public class GameEntry : MonoBehaviour
	{
		[SerializeField] private GameConfig gameConfig;
		[SerializeField] private FieldConfig fieldConfig;
		
		[Space]
		[SerializeField] private GameFieldView gameFieldView;
		[SerializeField] private MovesView movesView;
		[SerializeField] private ScoreView scoreView;
		[SerializeField] private BoosterPanelView boosterSwapView;
		[SerializeField] private BoosterPanelView boosterBombView;
		[SerializeField] private EndGamePopup endGamePopup;
		
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
				MaxMoves = gameConfig.maxMoves,
				MaxShuffles = gameConfig.maxShuffles,
				BoosterSwapStartCount = gameConfig.boosterSwapStartCount,
				BoosterBombStartCount = gameConfig.boosterBombStartCount,
				BoosterBombRadius = gameConfig.boosterBombRadius
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
				movesView,
				scoreView,
				boosterSwapView,
				boosterBombView,
				endGamePopup,
				tileViewLibrary, 
				tileViewPool, 
				gameConfigData,
				fieldConfigData);
		}
	}
}