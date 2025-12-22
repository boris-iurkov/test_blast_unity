using UnityEngine;

namespace Game.View.Config
{
	[CreateAssetMenu(menuName = "Game/Field Config")]
	public class FieldConfig : ScriptableObject
	{
		public int tileWidth = 100;
		public int tileHeight = 105;
		public int gameFieldWidth = 900;
		public int gameFieldHeight = 1000;
	}
}