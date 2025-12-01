using UnityEngine;
using UnityEngine.UI;

namespace GotchaNow
{
    [RequireComponent(typeof(Selectable))]
    public class ButtonTextHider : MonoBehaviour
	{
		[SerializeField] private GameObject textGameObject;
		// PUBLIC
		public void HideButtonText()
		{
			if (textGameObject != null)
			{
				textGameObject.SetActive(false);
			}
		}

		public void ShowButtonText()
		{
			if (textGameObject != null)
			{
				textGameObject.SetActive(true);
			}
		}

		// PRIVATE
		private void Awake()
		{
			if (textGameObject == null)
			{
				Debug.LogError("ButtonTextHider: Button Text reference is not assigned.", this);
			}
		}		
	}
}
