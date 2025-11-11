using UnityEngine;
using System.Collections;

namespace GotchaNow
{
	public class ChatMessagesManager : MonoBehaviour
	{
		[Header("Message Prefab")]
		[SerializeField] private ChatMessage messagePrefab;

		[Header("References")]
		[SerializeField] private Transform messageParent;

		public void CreateMessage(string messageText, float displayDuration)
		{
			StartCoroutine(ShowMessageAnimation(messageText, displayDuration));
		}

		//PRIVATE
		private void Awake()
		{
			if (messagePrefab == null)
			{
				throw new System.Exception("Message Prefab is not assigned in ChatMessagesManager.");
			}
			if (messageParent == null)
			{
				throw new System.Exception("Message Parent is not assigned in ChatMessagesManager.");
			}
		}
		
		private void Start()
		{
			// Example usage
			CreateMessage("Hello World!", 3f);
		}

		private IEnumerator ShowMessageAnimation(string messageText, float displayDuration)
		{
			ChatMessage messageScript = Instantiate(messagePrefab, messageParent);
			messageScript.messageText = messageText;

			Debug.Log("Message instantiated: " + messageScript.name);
			// Here you can add animation code if needed
			while (displayDuration > 0)
			{
				displayDuration -= Time.deltaTime;
				(messageScript.transform as RectTransform).anchoredPosition += 30 * Time.deltaTime * Vector2.down; // Example animation
				yield return null;
			}
			Destroy(messageScript.gameObject);
		}
	}
}
