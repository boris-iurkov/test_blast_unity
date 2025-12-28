using System;

namespace Game.Model
{
	public class MovesCounter : IMovesCounter
	{
		public int MaxMoves { get; private set; }
		public int MovesLeft { get; private set; }

		public event Action OnMovesChanged;

		public void Init(int maxMoves)
		{
			MaxMoves = maxMoves;
			MovesLeft = maxMoves;
		}

		public void MakeMove()
		{
			if (MovesLeft <= 0)
				return;
			
			MovesLeft--;
			
			FireMovesChanged();
		}

		private void FireMovesChanged()
		{
			OnMovesChanged?.Invoke();
		}
	}
}