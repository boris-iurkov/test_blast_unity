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
		}

		public void MakeMove()
		{
			if (MovesLeft <= 0)
				return;
			
			MovesLeft--;
			
			FireMovesChanged();
		}

		public void Reset()
		{
			MovesLeft = MaxMoves;
		}

		private void FireMovesChanged()
		{
			OnMovesChanged?.Invoke(MovesLeft);
		}
	}
}