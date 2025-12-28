using TMPro;
using UnityEngine;

namespace Game.View
{
	public class ScoreView : MonoBehaviour, IScoreView
	{
		[SerializeField] private TextMeshProUGUI labelCount;
		
		public void UpdateScoreCount(int count, int targetScore)
		{
			labelCount.SetText(count + "/" + targetScore);
		}
	}
}