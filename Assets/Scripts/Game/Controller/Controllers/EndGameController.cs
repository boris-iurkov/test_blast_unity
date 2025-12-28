using System;
using Game.Controller.Data;
using Game.Model;
using Game.View.Data;
using Game.View.Popup;

namespace Game.Controller.Controllers
{
	public class EndGameController
	{
		public bool IsEndGame => _isEndGame;
		public event Action OnRestartRequested;
		
		private IScoreCounter _scoreCounter;
		private IMovesCounter _movesCounter;
		private EndGamePopup _endGamePopup;
		
		private bool _isEndGame;
		private bool _isPopupShown;
		private EndGameResult _endGameResult = EndGameResult.None;
		private int _maxShuffles;
		private int _countShufflesMade;

		public void Init(IScoreCounter scoreCounter, IMovesCounter movesCounter, EndGamePopup endGamePopup, int maxShuffles)
		{
			_scoreCounter = scoreCounter;
			_movesCounter = movesCounter;
			_endGamePopup = endGamePopup;
			_maxShuffles = maxShuffles;
			
			_endGamePopup.gameObject.SetActive(false);
			_endGamePopup.OnButtonClicked += HandleEndGamePopupButtonClicked;
		}

		public void SetShufflesCount(int count)
		{
			_countShufflesMade = count;
		}

		public void TryEndGame()
		{
			if (_isEndGame)
				return;
			
			if (_scoreCounter.Score >= _scoreCounter.TargetScore)
			{
				_isEndGame = true;
				_endGameResult = EndGameResult.Win;
				return;
			}
			
			if (_movesCounter.MovesLeft <= 0)
			{
				_isEndGame = true;
				_endGameResult = EndGameResult.LoseNoMoves;
				return;
			}

			if (_countShufflesMade >= _maxShuffles)
			{
				_isEndGame = true;
				_endGameResult = EndGameResult.LoseNoTiles;
			}
		}

		public void ShowEndGamePopup()
		{
			if (!_isEndGame || _isPopupShown)
				return;
			
			_isPopupShown = true;

			_endGamePopup.gameObject.SetActive(true);
			EndGamePopupState state = GetEndGamePopupState(_endGameResult);
			_endGamePopup.Show(state);
		}

		public void Reset()
		{
			_isEndGame = false;
			_isPopupShown = false;
			_endGameResult = EndGameResult.None;
			_countShufflesMade = 0;
		}

		private EndGamePopupState GetEndGamePopupState(EndGameResult endGameResult)
		{
			switch (endGameResult)
			{
				case EndGameResult.Win:
					return EndGamePopupState.Win;
				
				case EndGameResult.LoseNoMoves:
					return EndGamePopupState.LoseNoMoves;
				
				case EndGameResult.LoseNoTiles:
					return EndGamePopupState.LoseNoTiles;
				
				default:
					return EndGamePopupState.Win;
			}
		}
		
		private void HandleEndGamePopupButtonClicked()
		{
			_endGamePopup.Hide(() =>
			{
				_endGamePopup.gameObject.SetActive(false);
				OnRestartRequested?.Invoke();
			});
		}
	}
}