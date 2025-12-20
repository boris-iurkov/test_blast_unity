using Game.Controller;
using Game.Model.Config;
using Game.View;
using UnityEngine;

namespace Game.Core
{
	public class GameEntry : MonoBehaviour
	{
		[SerializeField] private GameFieldView gameFieldView;
		[SerializeField] private GameConfig gameConfig;

		private GameController _gameController;

		private void Awake()
		{
			_gameController = new GameController();
			_gameController.Init(gameFieldView, gameConfig.RowsCount, gameConfig.ColumnsCount);
		}
	}
}