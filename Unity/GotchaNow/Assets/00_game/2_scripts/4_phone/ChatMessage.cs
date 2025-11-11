using UnityEngine;

namespace GotchaNow
{
	public class ChatMessage : MonoBehaviour
	{
		[SerializeField] private TMPro.TextMeshProUGUI messageTextUI;
		public string messageText
		{
			set
			{
				messageTextUI.text = value;
			}
		}

	}
}
