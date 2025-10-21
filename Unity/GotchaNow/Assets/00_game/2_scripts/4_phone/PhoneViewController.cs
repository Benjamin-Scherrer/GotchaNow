using UnityEngine;

namespace GotchaNow
{
	public class PhoneViewController : MonoBehaviour
	{
		public static PhoneViewController instance;

		[SerializeField] private RectTransform phoneViewTransform;

		private void Awake()
		{
			if (instance != null) throw new System.Exception("Multiple PhoneViewController instances detected");
			instance = this;
		}

		public void EnablePhoneView()
		{
			Debug.Log("Phone View Enabled");
			phoneViewTransform.gameObject.SetActive(true);
		}

	}
}
