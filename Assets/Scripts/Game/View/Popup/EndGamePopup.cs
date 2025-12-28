using System;
using DG.Tweening;
using Game.View.Config;
using Game.View.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View.Popup
{
	public class EndGamePopup : MonoBehaviour, IEndGamePopup
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

		public void SetActive(bool active)
		{
			gameObject.SetActive(active);
		}

		public void Show(EndGamePopupState state)
		{
			button.onClick.RemoveAllListeners();
			
			switch (state)
			{
				case EndGamePopupState.Win:
					title.SetText(endGamePopupConfig.titleWin);
					buttonText.SetText(endGamePopupConfig.buttonWin);
					break;
				
				case EndGamePopupState.LoseNoMoves:
					title.SetText(endGamePopupConfig.titleLoseNoMoves);
					buttonText.SetText(endGamePopupConfig.buttonLoseNoMoves);
					break;
				
				case EndGamePopupState.LoseNoTiles:
					title.SetText(endGamePopupConfig.titleLoseNoTiles);
					buttonText.SetText(endGamePopupConfig.buttonLoseNoTiles);
					break;
			}

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
			sequence.OnComplete(() =>
			{
				button.onClick.AddListener(HandleButtonClicked);
			});
		}

		public void Hide(Action onComplete = null)
		{
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
			button.onClick.RemoveAllListeners();
			OnButtonClicked?.Invoke();
		}
	}
}