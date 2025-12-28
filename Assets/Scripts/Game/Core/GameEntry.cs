using Game.Controller;
using Game.Controller.Controllers;
using Game.Controller.Factory;
using Game.Model;
using Game.Model.Booster;
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
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
			
			var gameConfigData = new GameConfigData
			{
				RowsCount = gameConfig.rowsCount,
				ColumnsCount = gameConfig.columnsCount,
				TargetScore = gameConfig.targetScore,
				MaxMoves = gameConfig.maxMoves,
				MaxShuffles = gameConfig.maxShuffles,
				BoosterSwapStartCount = gameConfig.boosterSwapStartCount,
				BoosterBombStartCount = gameConfig.boosterBombStartCount,
				BoosterBombRadius = gameConfig.boosterBombRadius,
				MinSuperTileGroupSize = gameConfig.minSuperTileGroupSize,
				RadiusSuperTileBombSmall = gameConfig.radiusSuperTileBombSmall
			};
			
			var fieldConfigData = new FieldConfigData
			{
				TileWidth = fieldConfig.tileWidth,
				TileHeight = fieldConfig.tileHeight,
				GameFieldWidth = fieldConfig.gameFieldWidth,
				GameFieldHeight = fieldConfig.gameFieldHeight
			};
			
			IGameField gameField = new GameField();
			gameField.Init(gameConfigData);
			
			IMovesCounter movesCounter = new MovesCounter();
			movesCounter.Init(gameConfigData.MaxMoves);
			
			IScoreCounter scoreCounter = new ScoreCounter();
			scoreCounter.Init(gameConfigData.TargetScore);
			
			IBoosterCounter boosterSwap = new BoosterCounter();
			boosterSwap.Init(gameConfigData.BoosterSwapStartCount);
			
			IBoosterBombCounter boosterBomb = new BoosterBombCounter();
			boosterBomb.Init(gameConfigData.BoosterBombStartCount, gameConfigData.BoosterBombRadius);
			
			gameFieldView.Init(
				tileViewLibrary,
				tileViewPool,
				fieldConfigData,
				gameField.RowsCount,
				gameField.ColumnsCount);
			
			for (var row = 0; row < gameField.RowsCount; row++)
			for (var column = 0; column < gameField.ColumnsCount; column++)
				gameFieldView.FillTile(gameField.GetTile(row, column));
			
			var endGameController = new EndGameController();
			endGameController.Init(scoreCounter, movesCounter, endGamePopup, gameConfigData.MaxShuffles);
			
			var shuffleController = new ShuffleController();
			shuffleController.Init(gameField, gameFieldView, endGameController, gameConfigData.MaxShuffles);
			
			var viewController = new ViewController();
			viewController.Init(movesView, scoreView, boosterSwapView, boosterBombView, movesCounter, scoreCounter, boosterSwap, boosterBomb);
			
			var boosterController = new BoosterController();
			boosterController.Init(boosterSwap, boosterBomb, boosterSwapView, boosterBombView, gameFieldView, gameField, viewController);
			
			var superTileFactory = new SuperTileFactory();
			
			var tileInteractionController = new TileInteractionController();
			tileInteractionController.Init(gameField, gameFieldView, scoreCounter, movesCounter, boosterController, superTileFactory);
			
			_gameController = new GameController();
			_gameController.Init(
				gameField,
				gameFieldView,
				movesCounter,
				scoreCounter,
				boosterSwap,
				boosterBomb,
				endGameController,
				shuffleController,
				viewController,
				boosterController,
				tileInteractionController);
			
			viewController.UpdateAllViews();
			boosterController.UpdateViews();
		}
	}
}