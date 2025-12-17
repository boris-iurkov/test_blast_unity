using Game.Model;
using UnityEngine;

namespace Game.Config
{
	[CreateAssetMenu(menuName = "Game/Tile View Config")]
	public class TileViewConfig : ScriptableObject
	{
		public TileColor color;
		public Sprite sprite;
	}
}