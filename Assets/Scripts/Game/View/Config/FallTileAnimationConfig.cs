using DG.Tweening;
using UnityEngine;

namespace Game.View.Config
{
	[CreateAssetMenu(menuName = "Game/Animation/Fall Tile Animation Config")]
	public class FallTileAnimationConfig : ScriptableObject
	{
		public float speed = 600f;
		public float startDelay = 0.05f;
		public float cascadeDelayRange = 0.2f;
		public Ease ease = Ease.Linear;
	}
}