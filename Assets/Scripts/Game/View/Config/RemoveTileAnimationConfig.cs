using DG.Tweening;
using UnityEngine;

namespace Game.View.Config
{
	[CreateAssetMenu(menuName = "Game/Animation/Remove Tile Animation Config")]
	public class RemoveTileAnimationConfig : ScriptableObject
	{
		public float scaleUp = 1.2f;
		public float scaleUpDuration = 0.12f;
		public float scaleDownDuration = 0.15f;
		public Ease scaleUpEase = Ease.OutQuad;
		public Ease scaleDownEase = Ease.InQuad;
	}
}