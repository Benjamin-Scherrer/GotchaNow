using UnityEngine;
using UnityEngine.UI;

namespace GotchaNow
{
	public class ButtonTextHider : MonoBehaviour
	{
		[SerializeField] private Text buttonText;
		private Button button;

		private void Awake()
		{
			button = GetComponent<Button>();
			
		}

		private void HideText()
		{
			buttonText.enabled = false;
		}
	}
}
