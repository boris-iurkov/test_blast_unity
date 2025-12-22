using Game.Model;
using Game.Model.Data;
using UnityEngine;

namespace Game.View.Config
{
	[CreateAssetMenu(menuName = "Game/Tile/Tile View Config")]
	public class TileViewConfig : ScriptableObject
	{
		public TileColor color;
		public Sprite sprite;
	}
}