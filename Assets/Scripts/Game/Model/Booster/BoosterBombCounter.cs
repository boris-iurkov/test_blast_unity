namespace Game.Model.Booster
{
	public class BoosterBombCounter : BoosterCounter, IBoosterBombCounter
	{
		public int Radius { get; private set; }

		public void Init(int startCount, int radius)
		{
			base.Init(startCount);

			Radius = radius;
		}
	}
}