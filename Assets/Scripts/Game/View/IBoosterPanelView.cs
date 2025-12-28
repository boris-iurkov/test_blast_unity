using System;

namespace Game.View
{
	public interface IBoosterPanelView
	{
		event Action OnBoosterClicked;
		
		void Init();
		void Select();
		void Unselect();
		void UpdateCount(int count);
	}
}