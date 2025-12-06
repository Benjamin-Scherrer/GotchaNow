using UnityEngine;

namespace GotchaNow
{
    [RequireComponent(typeof(Camera))]
    public class AspectRatioEnforcer : MonoBehaviour
	{
		public float targetAspectRatio = 1920f / 1080f;
		private Camera cam;

		private float screenAspectRatio => (float)Screen.width / Screen.height;
		private float lastScreenAspectRatio;

		void Start()
		{
			cam = GetComponent<Camera>();
			AdjustCameraRect();
		}

		void AdjustCameraRect()
		{
			float currentAspectRatio = (float)Screen.width / Screen.height;
			float scaleHeight = currentAspectRatio / targetAspectRatio;
			float scaleWidth = targetAspectRatio / currentAspectRatio;
			if (scaleHeight < 1.0f)
			{ // Screen is taller than target (letterbox needs to be used here)
				Rect letterbox = new(0, (1f - scaleHeight) * 0.5f, 1f, 1f / scaleWidth);
				cam.rect = letterbox;
				// Debug.Log("Applying letterbox: " + cam.rect);
			}
			else
			{ // Screen is wider than target (pillarbox needs to be used here)
				Rect pillarbox = new((1f - scaleWidth) * 0.5f, 0, 1f / scaleHeight, 1f);
				cam.rect = pillarbox;
				// Debug.Log("Applying pillarbox: " + cam.rect);
			}
		}

		// Call this if screen size changes during runtime (e.g., window resize)
		void Update()
		{
			if (lastScreenAspectRatio != screenAspectRatio && screenAspectRatio != targetAspectRatio)
			{
				// Debug.Log("Camera aspectr ration: " + screenAspectRatio + ", goal: " + targetAspectRatio);
				AdjustCameraRect();
				lastScreenAspectRatio = screenAspectRatio;
			}
		}
	}
}