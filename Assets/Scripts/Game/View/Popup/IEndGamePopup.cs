using System;
using Game.View.Data;

namespace Game.View.Popup
{
	public interface IEndGamePopup
	{
		event Action OnButtonClicked;
		
		void SetActive(bool active);
		void Show(EndGamePopupState state);
		void Hide(Action onComplete = null);
	}
}