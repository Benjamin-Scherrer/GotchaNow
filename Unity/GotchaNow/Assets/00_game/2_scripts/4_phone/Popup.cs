using System;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace GotchaNow
{
	public class Popup : MonoBehaviour
	{
		[SerializeField] private RectTransform popupExtra;
		[SerializeField] private RectTransform popupMain;

		[SerializeField] private RectTransform popupButton;

		//
		private bool jiggle;
		private float jiggleDuration;
		private float jiggleTime;
		private float jiggleIntensity;

		//
		private float buttonPressInDuration = 0.1f;
		private float buttonPressDuration = 0.2f;
		private float buttonPopOutDuration = 0.1f;
		private float buttonPressScale = 0.9f;

		public void StartJiggle(float jiggleDuration, float jiggleIntensity)
		{
			jiggle = true;
			this.jiggleDuration = jiggleDuration;
			jiggleTime = 0f;
			this.jiggleIntensity = jiggleIntensity;

			Debug.Log("Starting jiggle: duration " + jiggleDuration + ", intensity " + jiggleIntensity);
		}

		public void StartButtonPressAnimation(float buttonPressInDuration, float buttonPressDuration, float buttonPopOutDuration, float buttonPressScale)
		{
			this.buttonPressInDuration = buttonPressInDuration;
			this.buttonPressDuration = buttonPressDuration;
			this.buttonPopOutDuration = buttonPopOutDuration;
			this.buttonPressScale = buttonPressScale;
			StartCoroutine(AnimateButtonPress());
		}

		//PRIVATE METHODS
		private void Update()
		{
			Jiggle();
		}

		private IEnumerator AnimateButtonPress()
		{
			Debug.Log("Starting button press animation.");
			float buttonPressTime = 0f;
			// Press In
			while (buttonPressTime < buttonPressInDuration)
			{
				buttonPressTime += Time.deltaTime;
				float pressCoeff = buttonPressTime / buttonPressInDuration;
				float scaledValue = Mathf.Lerp(1f, buttonPressScale, pressCoeff);
				SetbuttonScale(scaledValue);
				yield return null;
			}
			// Hold Press
			buttonPressTime = 0f;
			while (buttonPressTime < buttonPressDuration)
			{
				buttonPressTime += Time.deltaTime;
				yield return null;
			}
			
			// Pop Out
			buttonPressTime = 0f;
			while (buttonPressTime < buttonPopOutDuration)
			{
				buttonPressTime += Time.deltaTime;
				float popOutCoeff = buttonPressTime / buttonPopOutDuration;
				float scaledValue = Mathf.Lerp(buttonPressScale, 1f, popOutCoeff);
				SetbuttonScale(scaledValue);
				yield return null;
			}

			// acceptPopup?.Invoke();
		}

		private void SetbuttonScale(float scale)
		{
			if (popupButton != null)
			{
				popupButton.localScale = Vector3.one * scale;
			}
		}

		private void Jiggle()
		{
			if (jiggle)
			{
				jiggleTime += Time.deltaTime;
				float jiggleCoeff = jiggleTime / jiggleDuration;
				float jiggleValue = Mathf.Sin(jiggleCoeff * Mathf.PI) * jiggleIntensity;
				SetJiggleValue(jiggleValue);
				Debug.Log("Jiggling with value: " + jiggleValue);
				Debug.Log("Jiggle time: " + jiggleTime + " / " + jiggleDuration);
				if (jiggleTime < jiggleDuration) return;
				Debug.Log("Jiggle finished.");
				jiggle = false;
				SetJiggleValue(0f);
			}
		}
		
		private void SetJiggleValue(float value)
		{
			if (popupExtra != null)
			{
				// popupExtra.localScale = Vector3.one * value;
				popupExtra.localRotation = Quaternion.Euler(0f, 0f, value);
			}
			if (popupMain != null)
			{
				// popupMain.localScale = Vector3.one * value;
				popupMain.localRotation = Quaternion.Euler(0f, 0f, value);
			}
			if (popupButton != null)
			{
				// popupButton.localScale = Vector3.one * value;
				popupButton.localRotation = Quaternion.Euler(0f, 0f, value);
			}
		}
	}
}
