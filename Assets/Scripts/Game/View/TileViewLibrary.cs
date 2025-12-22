using System.Collections.Generic;
using Game.Model;
using Game.Model.Data;
using Game.View.Config;
using UnityEngine;

namespace Game.View
{
	[CreateAssetMenu(menuName = "Game/Tile View Library")]
	public class TileViewLibrary : ScriptableObject
	{
		[SerializeField] private TileViewConfig[] configs;

		private Dictionary<TileColor, TileViewConfig> _map;

		private void OnEnable()
		{
			_map = new Dictionary<TileColor, TileViewConfig>();
			foreach (TileViewConfig config in configs)
			{
				_map[config.color] = config;
			}
		}

		public Sprite GetSprite(TileColor color)
		{
			if (_map.TryGetValue(color, out TileViewConfig config))
				return config.sprite;

			Debug.LogError($"No TileViewConfig for color: {color}");
			return null;
		}
	}
}