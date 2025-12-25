using System.Collections.Generic;

namespace Game.Model
{
	public class TileModelPool
	{
		private readonly Stack<TileModel> _stack = new();
		
		public TileModel GetTile()
		{
			TileModel tile = _stack.Count > 0 ? _stack.Pop() : CreateNewTile();
			return tile;
		}

		public void ReturnTile(TileModel tile)
		{
			tile.Reset();
			_stack.Push(tile);
		}
		
		private TileModel CreateNewTile()
		{
			return new TileModel();
		}
	}
}