using System;

namespace Game.Model
{
	public interface IMovesCounter
	{
		int MaxMoves { get; }
		int MovesLeft { get; }
		event Action OnMovesChanged;
		
		void Init(int maxMoves);
		void MakeMove();
	}
}