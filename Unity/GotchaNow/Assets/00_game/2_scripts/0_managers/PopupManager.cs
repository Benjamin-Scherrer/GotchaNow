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

		[Header("Popup Screens")]
		[SerializeField] private GameObject healMePopupPrefab;
		[SerializeField] private GameObject buffMePopupPrefab;
		[SerializeField] private GameObject meteoriteNowPopupPrefab;

		[Header("References")]
		[SerializeField] private Transform popupParent;

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
		}


		public void ShowPopup(PopupType popupType, float displayDuration)
		{
			switch (popupType)
			{
				case PopupType.HealMe:
					StartCoroutine(ShowPopupAnimation(healMePopupPrefab, displayDuration));
					break;
				case PopupType.BuffMe:
					StartCoroutine(ShowPopupAnimation(buffMePopupPrefab, displayDuration));
					break;
				case PopupType.MeteoriteNow:
					StartCoroutine(ShowPopupAnimation(meteoriteNowPopupPrefab, displayDuration));
					break;
				default:
					Debug.LogWarning("Unknown popup type: " + popupType);
					break;
			}
		}

		//PRIVATE
		private void Start()
		{
			// Example usage
			ShowPopup(PopupType.HealMe, 3f);
		}
		
		private IEnumerator ShowPopupAnimation(GameObject popupScreenPrefab, float displayDuration)
		{
			GameObject popupScreen = Instantiate(popupScreenPrefab, popupParent);
			Debug.Log("Popup instantiated: " + popupScreen.name);
			// Here you can add animation code if needed
			while (displayDuration > 0)
			{
				displayDuration -= Time.deltaTime;
				popupScreen.transform.Rotate(Vector3.up, 90 * Time.deltaTime); // Example animation
				yield return null;
			}
			Destroy(popupScreen);
		}
	}
}
