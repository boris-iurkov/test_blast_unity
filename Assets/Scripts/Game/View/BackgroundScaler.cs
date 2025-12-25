using UnityEngine;

namespace Game.View
{
	public class BackgroundScaler : MonoBehaviour
	{
		private const int RefWidth = 1080;
		private const int RefHeight = 1920;
		
		[SerializeField] private RectTransform rectTransformValue;

		private void Start()
		{
			Vector2 sizeBackgroundImage = GetSizeBackgroundImage();
			float backgroundImageWidth = sizeBackgroundImage.x;
			float backgroundImageHeight = sizeBackgroundImage.y;

			Vector2 sizeScreen = GetSizeScreen();
			float screenWidth = sizeScreen.x;
			float screenHeight = sizeScreen.y;
			
			float backgroundImageAspect = backgroundImageWidth / backgroundImageHeight;
			float screenAspect = screenWidth / screenHeight;

			if (screenAspect > backgroundImageAspect)
				rectTransformValue.sizeDelta = new Vector2(screenWidth, screenWidth / backgroundImageAspect);
			else
				rectTransformValue.sizeDelta = new Vector2(screenHeight * backgroundImageAspect, screenHeight);
		}

		private Vector2 GetSizeBackgroundImage()
		{
			return rectTransformValue.sizeDelta;
		}
		
		private Vector2 GetSizeScreen()
		{
			var mainCanvasRectTransform = transform.root as RectTransform;
			if (mainCanvasRectTransform != null)
				return mainCanvasRectTransform.sizeDelta;

			return new Vector2(RefWidth, RefHeight);
		}
	}
}