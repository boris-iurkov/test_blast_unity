using TMPro;
using UnityEngine;

namespace Game.View
{
	public class MovesView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI labelCount;
		
		public void UpdateMovesCount(int count)
		{
			labelCount.SetText(count.ToString());
		}
	}
}