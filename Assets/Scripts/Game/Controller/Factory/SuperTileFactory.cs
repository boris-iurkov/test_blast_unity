using Game.Model.SuperTile;
using UnityEngine;

namespace Game.Controller.Factory
{
	public class SuperTileFactory
	{
		public ISuperTileLogic CreateRandomSuperTile()
		{
			int rnd = Random.Range(1, 5);
			switch (rnd)
			{
				case 1:
					return new SuperTileLogicRow();
				
				case 2:
					return new SuperTileLogicColumn();
				
				case 3:
					return new SuperTileLogicExplodeSmall();

				default:
					return new SuperTileLogicExplodeBig();
			}
		}
	}
}