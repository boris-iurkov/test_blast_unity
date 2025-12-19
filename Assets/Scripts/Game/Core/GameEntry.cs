using Game.Controller;
using Game.View;
using UnityEngine;

namespace Game.Core
{
	public class GameEntry : MonoBehaviour
	{
		[SerializeField] private BoardView boardView;
		
		private GameController _gameController;

		private void Awake()
		{
			_gameController = new GameController();
			_gameController.Init(boardView);
		}
	}
}