using System;
using System.Collections;
using UnityEngine;

namespace GotchaNow
{
	public class PopupManager : MonoBehaviour
	{
		public enum PopupType
		{
			HealMe,
			BuffMe,
			MeteoriteNow
		}

		public static PopupManager instance;

		[Header("Popup Screens")]
		[SerializeField] private Popup healMePopupPrefab;
		[SerializeField] private Popup buffMePopupPrefab;
		[SerializeField] private Popup meteoriteNowPopupPrefab;

		[Header("References")]
		[SerializeField] private Transform popupParent;

		[Header("Variables")]
		[SerializeField] private float popUpDuration = 0.5f;
		[SerializeField] private float displayDuration = 2f;
		[SerializeField] private float popDownDuration = 0.5f;

		[SerializeField] private float jiggleInterval = 0.1f;
		[SerializeField] private int jiggleAmount = 5;
		[SerializeField] private float jiggleDuration = 0.5f;
		[SerializeField] private float jiggleIntensity = 1f;


		private void Awake()
		{
			if (healMePopupPrefab == null)
			{
				throw new System.Exception("Heal Me Popup is not assigned in PopupManager.");
			}
			if (buffMePopupPrefab == null)
			{
				throw new System.Exception("Buff Me Popup is not assigned in PopupManager.");
			}
			if (meteoriteNowPopupPrefab == null)
			{
				throw new System.Exception("Meteorite Now Popup is not assigned in PopupManager.");
			}
			if(popupParent == null)
			{
				throw new System.Exception("Popup Parent is not assigned in PopupManager.");
			}
			if (instance != null)
            {
				throw new System.Exception("Multiple instances of PopupManager detected. There should only be one instance of PopupManager in the scene.");
            }
			instance = this;
		}

		public void ShowHealMePopup()
		{
			StartCoroutine(ShowPopupAnimation(healMePopupPrefab, () =>
            {
                NotificationManager.instance.AcceptHeal();
            }));
		}

		public void ShowBuffMePopup()
		{
			StartCoroutine(ShowPopupAnimation(buffMePopupPrefab, () =>
            {
                NotificationManager.instance.AcceptBuff();
            }));
		}

		public void ShowMeteoriteNowPopup()
		{
			StartCoroutine(ShowPopupAnimation(meteoriteNowPopupPrefab, () =>
            {
                NotificationManager.instance.AcceptMeteor();
            }));
		}

		//PRIVATE
		private void Start()
		{
			// Example usage
			// ShowPopup(PopupType.HealMe);
			// StartCoroutine(LoopSpawnPopups());
		}


		// private IEnumerator LoopSpawnPopups()
		// {
		// 	WaitForSeconds wait = new(0.1f);
		// 	while (true)
		// 	{
		// 		yield return StartCoroutine(ShowPopupAnimation(healMePopupPrefab));
		// 		yield return wait;
		// 		yield return StartCoroutine(ShowPopupAnimation(buffMePopupPrefab));
		// 		yield return wait;
		// 		yield return StartCoroutine(ShowPopupAnimation(meteoriteNowPopupPrefab));
		// 		yield return wait;
		// 	}
		// }
		
		private IEnumerator ShowPopupAnimation(Popup popupScreenPrefab, Action acceptPopup = null)
		{
			Popup popupScreen = Instantiate(popupScreenPrefab, popupParent);
			Debug.Log("Popup instantiated: " + popupScreen.name);
			// Here you can add animation code if needed

			float popUpTime = 0f;

			while (popUpTime < popUpDuration)
			{
				popUpTime += Time.deltaTime;
				float popupfCoeff = popUpTime / popUpDuration;
				popupScreen.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, popupfCoeff);
				yield return null;
			}

			int performedJiggles = 0;
			float jiggleTime = 0;

			float displayTime = 0f;
			while (displayTime < displayDuration)
			{
				displayTime += Time.deltaTime;

				jiggleTime += Time.deltaTime;
				if(jiggleTime >= jiggleInterval && performedJiggles < jiggleAmount)
				{
					jiggleTime -= jiggleInterval;
					performedJiggles++;
					popupScreen.StartJiggle(jiggleDuration, jiggleIntensity);
				}

				yield return null;
			}

			acceptPopup?.Invoke();

			float popDownTime = 0f;
			while (popDownTime < popDownDuration)
			{
				popDownTime += Time.deltaTime;
				float popupfCoeff = 1 - (popDownTime / popDownDuration);
				popupScreen.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, popupfCoeff);
				yield return null;
			}
			Destroy(popupScreen);
		}
	}
}
