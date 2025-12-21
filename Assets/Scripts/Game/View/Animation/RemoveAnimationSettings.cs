using DG.Tweening;

namespace Game.View.Animation
{
	[System.Serializable]
	public class RemoveAnimationSettings
	{
		public float scaleUp = 1.2f;
		public float scaleUpDuration = 0.12f;
		public float scaleDownDuration = 0.15f;
		public Ease scaleUpEase = Ease.OutQuad;
		public Ease scaleDownEase = Ease.InQuad;
	}
}