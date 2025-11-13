using UnityEngine;

namespace GotchaNow
{
	public class Popup : MonoBehaviour
	{
		[SerializeField] private RectTransform popupExtra;
		[SerializeField] private RectTransform popupMain;

		[SerializeField] private RectTransform popupButton;

		private bool jiggle;
		private float jiggleDuration;
		private float jiggleTime;
		private float jiggleIntensity;
		public void StartJiggle(float jiggleDuration, float jiggleIntensity)
		{
			jiggle = true;
			this.jiggleDuration = jiggleDuration;
			jiggleTime = 0f;
			this.jiggleIntensity = jiggleIntensity;

			Debug.Log("Starting jiggle: duration " + jiggleDuration + ", intensity " + jiggleIntensity);
		}

		//PRIVATE METHODS
		private void Update()
		{
			Jiggle();
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
