using System.Collections;
using UnityEngine;
using UnityEssentials;

namespace GotchaNow
{
	public class PhoneManager : MonoBehaviour
	{
		public static PhoneManager Instance;

		[Header("References")]
		[SerializeField] private RectTransform phoneTransform;
		[Header("Jiggle Settings")]
		[SerializeField] private float jiggleOscillations = 5f;
		[SerializeField] private float jiggleDuration = 0.5f;
		[SerializeField] private float jiggleIntensity = 5f;

		private Quaternion originalRotation;
		public void JigglePhone()
		{
			StartCoroutine(JigglePhoneCoroutine());
		}

		// PRIVATE
		private void Awake()
		{
			if (Instance != null) throw new System.Exception("Multiple PhoneJiggler instances detected!");
			Instance = this;
			originalRotation = phoneTransform.localRotation;
		}

		private IEnumerator JigglePhoneCoroutine()
		{
			Debug.Log("Starting phone jiggle...");
			float elapsed = 0f;
			float jiggles = 0f;

			while (jiggles < jiggleOscillations)
			{
				float localDuration = jiggleDuration / jiggleOscillations;
				while (elapsed < localDuration)
				{
					elapsed += Time.unscaledDeltaTime;
					float normalizedTime = Mathf.Clamp01(elapsed / localDuration);
					float angle = Mathf.Sin(normalizedTime * Mathf.PI * 2f) * jiggleIntensity;
					phoneTransform.localRotation = Quaternion.Euler(0f, 0f, 0f + angle);
					yield return null;
				}
				jiggles += 1f;
				elapsed = 0f;
			}
			phoneTransform.localRotation = originalRotation;
			Debug.Log("Phone jiggle completed.");
		}
	}
}
