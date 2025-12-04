using System;
using System.Collections;
using NUnit.Framework.Constraints;
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
		[SerializeField] private ParticleSystem[] popupParticles;

		[Header("Variables")]
		[Header("Popup Animation Settings")]
		[SerializeField] private float popUpDuration = 0.5f;
		[SerializeField] private float displayDuration = 2f;
		[SerializeField] private float popDownDuration = 0.5f;

		[Header("Jiggle Settings")]
		[SerializeField] private float jiggleInterval = 0.1f;
		[SerializeField] private int jiggleAmount = 5;
		[SerializeField] private float jiggleDuration = 0.5f;
		[SerializeField] private float jiggleIntensity = 1f;

		[Header("Button Press Animation Settings")]
		[SerializeField] private float buttonPressInDuration = 0.1f;
		[SerializeField] private float buttonPressDuration = 0.2f;
		[SerializeField] private float buttonPopOutDuration = 0.1f;
		[SerializeField] private float buttonPressScale = 0.9f;
		[SerializeField] private float buttonPressPopupPopdownDelay = 0.1f;


		//PUBLIC
		public void EnableParticles()
        {
			foreach(ParticleSystem ps in popupParticles)
			{
				ps.gameObject.SetActive(true);
			}
        }

		public void DisableParticles()
		{
			foreach(ParticleSystem ps in popupParticles)
			{
				ps.gameObject.SetActive(false);
			}
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
			if(!buffMePopupPrefab) throw new Exception("No buff me prefab");
			if(!NotificationManager.instance) throw new Exception("WTF");
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
		private void Awake()
		{
			if (healMePopupPrefab == null)
			{
				throw new Exception("Heal Me Popup is not assigned in PopupManager.");
			}
			if (buffMePopupPrefab == null)
			{
				throw new Exception("Buff Me Popup is not assigned in PopupManager.");
			}
			if (meteoriteNowPopupPrefab == null)
			{
				throw new Exception("Meteorite Now Popup is not assigned in PopupManager.");
			}
			if(popupParent == null)
			{
				throw new Exception("Popup Parent is not assigned in PopupManager.");
			}
			if (instance != null)
            {
				throw new Exception("Multiple instances of PopupManager detected. There should only be one instance of PopupManager in the scene.");
            }
			instance = this;

			DisableParticles();
		}
		// private void Start()
		// {
		// 	// Example usage
		// 	// ShowPopup(PopupType.HealMe);
		// 	// StartCoroutine(LoopSpawnPopups());
		// }


		// private IEnumerator LoopSpawnPopups()
		// {
		// 	WaitForSecondsRealtime wait = new(0.1f);
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
			WaitForSecondsRealtime waitForSeconds = new(1f);

			Popup popupScreen = Instantiate(popupScreenPrefab, popupParent);
			Debug.Log("Popup instantiated: " + popupScreen.name);
			// Here you can add animation code if needed

			//Jiggle phone
			PhoneManager.Instance.JigglePhone();

			float popUpTime = 0f;

			while (popUpTime < popUpDuration)
			{
				if(PauseMenu.Instance.IsPaused)
				{
					Debug.Log("Waiting for unpause");
					yield return waitForSeconds;
					continue;
				}

				popUpTime += Time.unscaledDeltaTime;
				float popupfCoeff = popUpTime / popUpDuration;
				popupScreen.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, popupfCoeff);
				yield return null;
			}

			int performedJiggles = 0;
			float jiggleTime = 0;

			float displayTime = 0f;
			while (displayTime < displayDuration)
			{
				if(PauseMenu.Instance.IsPaused)
				{
					Debug.Log("Waiting for unpause 2");
					yield return waitForSeconds;
					continue;
				}

				displayTime += Time.unscaledDeltaTime;

				jiggleTime += Time.unscaledDeltaTime;
				if(jiggleTime >= jiggleInterval && performedJiggles < jiggleAmount)
				{
					jiggleTime -= jiggleInterval;
					performedJiggles++;
					popupScreen.StartJiggle(jiggleDuration, jiggleIntensity);
				}
				
				yield return null;
			}

            popupScreen.StartButtonPressAnimation(buttonPressInDuration, buttonPressDuration, 
				buttonPopOutDuration, buttonPressScale);
                
			acceptPopup?.Invoke();
			
			yield return new WaitForSecondsRealtime(buttonPressInDuration 
				+ buttonPressDuration
				+ buttonPopOutDuration
				+ buttonPressPopupPopdownDelay);

			float popDownTime = 0f;
			while (popDownTime < popDownDuration)
			{
				if(PauseMenu.Instance.IsPaused)
				{
					Debug.Log("Waiting for unpause3");
					yield return waitForSeconds;
					continue;
				}

				popDownTime += Time.unscaledDeltaTime;
				float popupfCoeff = 1 - (popDownTime / popDownDuration);
				popupScreen.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, popupfCoeff);
				yield return null;
			}
			Destroy(popupScreen);
		}
	}
}
