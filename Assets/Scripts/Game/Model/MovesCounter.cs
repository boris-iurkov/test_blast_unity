using System;

namespace Game.Model
{
	public class MovesCounter
	{
		public int MaxMoves { get; private set; }
		public int MovesLeft { get; private set; }

		public event Action<int> OnMovesChanged;

		public void Init(int maxMoves)
		{
			MaxMoves = maxMoves;
			MovesLeft = maxMoves;
			OnMovesChanged?.Invoke(MovesLeft);
		}

		public void MakeMove()
		{
			if (MovesLeft <= 0)
				return;
			
			MovesLeft--;
			
			OnMovesChanged?.Invoke(MovesLeft);
		}
	}
}