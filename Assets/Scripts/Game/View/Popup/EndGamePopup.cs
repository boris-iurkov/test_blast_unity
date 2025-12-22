using System;
using DG.Tweening;
using Game.View.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View.Popup
{
	public class EndGamePopup : MonoBehaviour
	{
		[SerializeField] private EndGamePopupConfig endGamePopupConfig;
		
		[Space]
		[SerializeField] private CanvasGroup background;
		[SerializeField] private RectTransform content;
		
		[Space]
		[SerializeField] private TextMeshProUGUI title;
		[SerializeField] private Button button;
		[SerializeField] private TextMeshProUGUI buttonText;

		public event Action OnButtonClicked;

		public void Show(bool win)
		{
			if (win)
			{
				title.SetText(endGamePopupConfig.titleWin);
				buttonText.SetText(endGamePopupConfig.buttonWin);
			}
			else
			{
				title.SetText(endGamePopupConfig.titleLose);
				buttonText.SetText(endGamePopupConfig.buttonLose);
			}

			button.onClick.AddListener(HandleButtonClicked);
			
			content.localScale = Vector3.zero;
			background.alpha = 0;
			
			Sequence sequence = DOTween.Sequence();
			sequence.Append(
				background.DOFade(1f, endGamePopupConfig.backgroundFadeDuration)
				);
			sequence.Append(
				content.DOScale(1f, endGamePopupConfig.contentScaleDuration)
					.SetEase(endGamePopupConfig.contentShowEase)
				);
		}

		public void Hide(Action onComplete = null)
		{
			button.onClick.RemoveListener(HandleButtonClicked);
			
			Sequence sequence = DOTween.Sequence();
			sequence.Append(
				content.DOScale(0f, endGamePopupConfig.contentScaleDuration)
					.SetEase(endGamePopupConfig.contentHideEase)
				);
			sequence.Append(
				background.DOFade(0f, endGamePopupConfig.backgroundFadeDuration)
				);
			sequence.OnComplete(() =>
			{
				onComplete?.Invoke();
			});
		}

		private void HandleButtonClicked()
		{
			OnButtonClicked?.Invoke();
		}
	}
}