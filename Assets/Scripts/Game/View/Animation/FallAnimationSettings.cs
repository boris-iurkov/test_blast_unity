using DG.Tweening;

namespace Game.View.Animation
{
	[System.Serializable]
	public class FallAnimationSettings
	{
		public float speed = 600f;
		public float startDelay = 0.05f;
		public float cascadeDelayRange = 0.2f;
		public Ease ease = Ease.Linear;
	}
}