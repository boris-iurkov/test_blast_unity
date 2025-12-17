using System;
using Game.Controller;
using UnityEngine;

namespace Game
{
	public class GameEntry : MonoBehaviour
	{
		private GameController _gameController;

		private void Awake()
		{
			_gameController = new GameController();
		}
	}
}