using DG.Tweening;
using UnityEngine;

namespace Game.View.Config
{
	[CreateAssetMenu(menuName = "Game/Animation/Shuffle Animation Config")]
	public class ShuffleAnimationConfig : ScriptableObject
	{
		public float stepDelay = 0.01f;
		public int speed = 1000;
		public Ease moveEase = Ease.InOutQuad;
	}
}