using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
	public class BoosterPanelView : MonoBehaviour, IBoosterPanelView
	{
		[SerializeField] private Button button;
		[SerializeField] private TextMeshProUGUI labelCount;
		[SerializeField] private RectTransform rectTransform;

		public event Action OnBoosterClicked;

		public void Init()
		{
			button.onClick.AddListener(HandleButtonClicked);
		}

		public void Select()
		{
			rectTransform.DOKill(false);
			rectTransform.DOScale(new Vector3(1.1f, 1.1f), 0.3f);
		}
		
		public void Unselect()
		{
			rectTransform.DOKill(false);
			rectTransform.DOScale(Vector3.one, 0.3f);
		}

		public void UpdateCount(int count)
		{
			labelCount.SetText(count.ToString());
		}

		private void HandleButtonClicked()
		{
			OnBoosterClicked?.Invoke();
		}
	}
}