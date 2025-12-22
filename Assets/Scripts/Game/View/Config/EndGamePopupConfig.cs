using DG.Tweening;
using UnityEngine;

namespace Game.View.Config
{
	[CreateAssetMenu(menuName = "Game/End Game Popup Config")]
	public class EndGamePopupConfig : ScriptableObject
	{
		public string titleWin = "Победа";
		public string buttonWin = "Заново";
		
		[Space]
		public string titleLoseNoMoves = "Ходов нет";
		public string buttonLoseNoMoves = "Еще раз";
		
		[Space]
		public string titleLoseNoTiles = "Групп тайлов нет";
		public string buttonLoseNoTiles = "Рестарт";

		[Space] 
		public float backgroundFadeDuration = 0.2f;
		public float contentScaleDuration = 0.3f;
		public Ease contentShowEase = Ease.OutBack;
		public Ease contentHideEase = Ease.InBack;
	}
}