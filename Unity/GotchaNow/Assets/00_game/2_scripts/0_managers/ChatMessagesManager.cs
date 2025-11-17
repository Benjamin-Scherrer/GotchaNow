using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEssentials;

namespace GotchaNow
{
	[RequireComponent(typeof(ChatMessageSelector))]
	public class ChatMessagesManager : MonoBehaviour
	{
		public static ChatMessagesManager Instance;

		[Header("Message Prefab")]
		[SerializeField] private ChatMessage messagePrefab;

		[Header("References")]
		[SerializeField] private Transform messageParent;
		[SerializeField] private Transform messageSpawnPoint;
		// [SerializeField] private Transform messageGoal;

		[Header("Variables")]
		// [SerializeField] private float messageSpeed = 512f;
		[SerializeField] private float popUpDuration = 0.5f;
		[SerializeField] private float messageSpacing = 10f;
		[SerializeField] private float swipeAwayDuration = 0.3f;
		[SerializeField] private int swipeTreshold = 3;

		private ChatMessageSelector messageSelector;
		[SerializeField] private List<ChatMessage> activeMessages = new();

		//PUBLIC
		public void DisplayMessageHistory()
		{
			Debug.Log("Displaying message history.");
			StartCoroutine(ShowChatMessageHistory(messageSelector.GetChatMessageHistory()));
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

			messageSelector = GetComponent<ChatMessageSelector>();
			if (messageSelector == null)
			{
				throw new System.Exception("ChatMessageSelector component is missing from ChatMessagesManager GameObject.");
			}

			if (Instance != null)
			{
				throw new System.Exception("Multiple instances of ChatMessagesManager detected. There should only be one instance of ChatMessagesManager in the scene.");
			}
			Instance = this;
		}

		// private void Start()
		// {
		// 	// Example usage
		// 	// SpawnMessageHistory(messageSelector.GetChatMessageHistory());

		// 	StartCoroutine(LoopSpawnMessages());
		// }

		// private IEnumerator LoopSpawnMessages()
		// {
        //     while (true)
        //     {
		// 		yield return StartCoroutine(ShowChatMessageHistory(messageSelector.GetChatMessageHistory()));	
        //     }
		// }

		// private void SpawnMessageHistory(ChatMessageHistory chatMessageHistory)
		// {
		// 	if (chatMessageHistory == null)
		// 	{
		// 		Debug.LogWarning("Attempted to spawn a null ChatMessageHistory.");
		// 		return;
		// 	}
		// 	StartCoroutine(ShowChatMessageHistory(chatMessageHistory));
		// }

		private IEnumerator ShowChatMessageHistory(ChatMessageHistory chatMessageHistory)
		{
			yield return new WaitForSeconds(1f); // initial delay before starting chat message history
			chatMessageHistory.ResetChatHistory();
			ChatMessageData messageData;
			while ((messageData = chatMessageHistory.GetNextMessage()) != null)
			{
				float delayTillNext = messageData.DelayTillNext;
				float displayDuration = messageData.DisplayDuration;
				StartCoroutine(ShowMessageAnimation(messageData, displayDuration));
				if(activeMessages.Count >= swipeTreshold)
				{
					StartCoroutine(SwipeCascade());
				}
				yield return new WaitForSeconds(delayTillNext);
			}

			// clean up after all messages have been shown
			chatMessageHistory.ResetChatHistory();
			StartCoroutine(SwipeCascade());
		}

		private IEnumerator SwipeCascade()
        {
			WaitForSeconds waitForSeconds = new(0.2f);
            for (int i = 0; i < activeMessages.Count; i++)
			{
				ChatMessage msg = activeMessages[i];
				if (msg == null) 
				{
					activeMessages.Remove(msg);
					continue;
				}
				if(msg.Written == false)
				{
					continue;
				}
				StartCoroutine(SwipeAwayMessage(msg, swipeAwayDuration));
				yield return waitForSeconds;
			}
        }

		private IEnumerator ShowMessageAnimation(ChatMessageData messageData, float displayDuration = float.MaxValue)
		{
			if (messageData == null)
			{
				Debug.LogWarning("Attempted to show a null ChatMessageData.");
				yield break;
			}
			float typingPreview = messageData.TypingPreview;
			Vector3 startScale = new(0.5f, 0.3f, 1f);

			ChatMessage messageScript = Instantiate(messagePrefab, messageParent as RectTransform);
			messageScript.InitializeMessage(messageData);
			activeMessages.Add(messageScript);
			Debug.Log("Starting animation for message: " + messageScript.name);

			RectTransform messageRect = messageScript.transform as RectTransform;
			RectTransform spawnRect = messageSpawnPoint as RectTransform;
			// RectTransform goalRect = messageGoal as RectTransform;


			//Set initial position
			messageRect.anchoredPosition = spawnRect.anchoredPosition;

			// Typing Preview
			messageScript.ScaleMessageRespectingFontSize(startScale);
			messageScript.SetSenderText(messageData.GetSenderName + " is typing...");
			messageScript.SetMessageText("");
			PushDown();
			while (typingPreview > 0f)
			{
				typingPreview -= Time.deltaTime;
				if (messageScript == null || messageScript.gameObject == null)
				{
					yield break;
				}
				yield return null;
			}

			float elapsedTime = 0f;
			Debug.Log("Message instantiated: " + messageScript.name);
			messageScript.SetSenderText(messageData.GetSenderName);
			messageScript.SetMessageText(messageData.MessageContent);

			messageScript.Written = true;
			// Here you can add animation code if needed
			while (elapsedTime < popUpDuration)
			{
				elapsedTime += Time.deltaTime;
				float timeCoefficient = elapsedTime / popUpDuration;

				Vector3 scaleValue = Vector3.Lerp(startScale, Vector3.one, timeCoefficient);
				messageScript.ScaleMessageRespectingFontSize(scaleValue);
				PushDown();

				if (messageScript == null || messageScript.gameObject == null)
				{
					yield break;
				}
				yield return null;
			}
			while (elapsedTime < displayDuration)
			{
				elapsedTime += Time.deltaTime;
				if (messageScript == null || messageScript.gameObject == null)
				{
					yield break;
				}
				yield return null;
			}

			StartCoroutine(SwipeAwayMessage(messageScript, swipeAwayDuration));
		}

		private IEnumerator SwipeAwayMessage(ChatMessage messageScript, float duration)
		{

			if (messageScript == null)
			{
				yield break;
			}
			if(messageScript.GettingSwipedAway)
			{
				yield break; // already being swiped away
			}
			messageScript.GettingSwipedAway = true;

			RectTransform messageRect = messageScript.transform as RectTransform;
			Vector2 startPos = messageRect.anchoredPosition;
			Vector2 endPos = startPos + new Vector2(
				messageRect.rect.width * 2,
				0f
			);

			float elapsedTime = 0f;
			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float timeCoefficient = elapsedTime / duration;

				messageRect.anchoredPosition = Vector2.Lerp(startPos, endPos, timeCoefficient);
				yield return null;
			}

			activeMessages.Remove(messageScript);
			Destroy(messageScript.gameObject);
		}

		private void PushDown()
		{
			int messageCount = activeMessages.Count;
			if (messageCount <= 1) return;

			float yStart = (messageSpawnPoint as RectTransform).anchoredPosition.y;
			float yOffset = yStart;
			for (int i = messageCount - 1; i >= 0; i--)
			{
				ChatMessage currentMessage = activeMessages[i];
	
				if (currentMessage == null) continue;
				if (currentMessage.enabled == false) continue;
				if (currentMessage.gameObject.activeInHierarchy == false) continue;

				RectTransform messageRect = currentMessage.transform as RectTransform;
				messageRect.anchoredPosition = new Vector2(
					messageRect.anchoredPosition.x,
					yOffset
				);
				yOffset -= currentMessage.getHeight + messageSpacing;
			}
		}
	}
}