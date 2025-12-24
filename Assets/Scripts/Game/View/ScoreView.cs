using TMPro;
using UnityEngine;

namespace Game.View
{
	public class ScoreView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI labelCount;
		
		public void UpdateScoreCount(int count, int targetScore)
		{
			labelCount.SetText(count + "/" + targetScore);
		}
	}
}