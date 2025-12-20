using Game.Controller;
using Game.View;
using UnityEngine;

namespace Game.Core
{
	public class GameEntry : MonoBehaviour
	{
		[SerializeField] private GameFieldView gameFieldView;
		
		private GameController _gameController;

		private void Awake()
		{
			_gameController = new GameController();
			_gameController.Init(gameFieldView);
		}
	}
}