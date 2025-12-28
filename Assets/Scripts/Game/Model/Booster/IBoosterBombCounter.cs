namespace Game.Model.Booster
{
	public interface IBoosterBombCounter : IBoosterCounter
	{
		int Radius { get; }
		
		void Init(int startCount, int radius);
	}
}