using System;
using System.Collections;
using UnityEngine;

namespace GotchaNow
{
	public class TrueEndingOverlay : MonoBehaviour
	{
		public static TrueEndingOverlay Instance;

		[Header("References")]
		[SerializeField] private GameObject trueEndingOverlay;
		[SerializeField] private RectTransform messageOverlay;

		[Header("Variables")]
		[SerializeField] private float popUpTime;
		// x: moveTime, y: position, z: delay
		[SerializeField] private Vector3[] chatBoxVariables;


		// PUBLIC
		public void ActivateTrueEndingPhoneOverlayWithCallback(Action onComplete)
		{
			ProgressionManager progressionManager = ProgressionManager.instance;
			bool check1 = progressionManager != null;
			bool check2 = progressionManager.gameState == "intermission";
			bool check3 = progressionManager.intermissionID == "trueEnding";
			if(!(check1 && check2 && check3))
			{
				onComplete?.Invoke();
				return;
			}
			StartCoroutine(StartOverlayAnimationWithCallback(onComplete));
		}

		public void ActivateTrueEndingPhoneOverlay()
        {
			ProgressionManager progressionManager = ProgressionManager.instance;
			bool check1 = progressionManager != null;
			bool check2 = progressionManager.gameState == "intermission";
			bool check3 = progressionManager.intermissionID == "trueEnding";
            if(!(check1 && check2 && check3))
            {
                return;
            }
			StartCoroutine(StartOverlayAnimation());
		}

		// PRIVATE
		private void Awake()
        {
			if(Instance != null) throw new System.Exception("Multiple TrueEndingOverlay instances detected!");
			Instance = this;

			trueEndingOverlay.SetActive(false);

			// Test start:
			// StartCoroutine(StartOverlayAnimation());
        }

		private IEnumerator StartOverlayAnimationWithCallback(Action onComplete)
		{
			yield return StartCoroutine(StartOverlayAnimation());
			onComplete?.Invoke();
		}

		private IEnumerator StartOverlayAnimation()
		{
			Debug.Log("Starting True Ending Overlay Coroutine");
			// TODO: Grey out game screen and show true ending phone overlay

			trueEndingOverlay.SetActive(true);
			RectTransform trueEndingRect = trueEndingOverlay.GetComponent<RectTransform>();
			trueEndingRect.localScale = Vector3.zero;
			float time = 0f;

			// Vector2 trueEndingAnchoredPos = trueEndingRect.anchoredPosition;
			// Vector2 popupPos = new Vector2(trueEndingAnchoredPos.x, trueEndingAnchoredPos.y + 720f);
			Debug.Log("Scaling overlay up...");
			while(time < popUpTime)
			{
				time += Time.unscaledDeltaTime;
				float normalizedTime = Mathf.Clamp01(time / popUpTime);
				float scaleValue = Mathf.SmoothStep(0f, 1f, normalizedTime);
				trueEndingRect.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
				Debug.Log("Scaling overlay: " + scaleValue);

				// Move position up
				// float newYPos = Mathf.SmoothStep(trueEndingAnchoredPos.y, popupPos.y, normalizedTime);
				// trueEndingRect.anchoredPosition = new Vector2(trueEndingAnchoredPos.x, newYPos);

				yield return null;
			}
			Debug.Log("Overlay scaled to full size.");
		
			foreach(Vector3 chatBoxVariable in chatBoxVariables)
			{
				float previousYPos = messageOverlay.anchoredPosition.y;
				time = 0f;

				float moveTime = chatBoxVariable.x;
				float positionY = chatBoxVariable.y;
				while(time < moveTime)
				{
					time += Time.unscaledDeltaTime;
					float normalizedTime = Mathf.Clamp01(time / moveTime);
					float newYPos = Mathf.Lerp(previousYPos, positionY, normalizedTime);
					messageOverlay.anchoredPosition = new Vector2(messageOverlay.anchoredPosition.x, newYPos);
					yield return null;
				}
				previousYPos = positionY;
				float delay = chatBoxVariable.z;
				yield return new WaitForSecondsRealtime(delay);
			}

			ThanksForPlaying.Instance.ShowThanksScreen();
		}
	}
}
