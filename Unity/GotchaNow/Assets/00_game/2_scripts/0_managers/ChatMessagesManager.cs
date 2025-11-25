using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GotchaNow
{
	[RequireComponent(typeof(ChatMessageSelector))]
	[RequireComponent(typeof(ChatMessageImageSelector))]
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
		private ChatMessageImageSelector messageImageSelector;
		[SerializeField] private List<ChatMessage> activeMessages = new();

		//PUBLIC
		public void DisplayMessageHistory()
		{
			Debug.Log("Trying to display message history.");
			ChatMessageHistory messageHistory = messageSelector.GetChatMessageHistory();
			if (messageHistory == null)
			{
				Debug.Log("No ChatMessageHistory found to display.");
				return;
			}
			Debug.Log("Displaying message history.");
			StartCoroutine(QueueChatMessageHistory(messageHistory));
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

			messageImageSelector = GetComponent<ChatMessageImageSelector>();
			if (messageImageSelector == null)
			{
				throw new System.Exception("ChatMessageImageSelector component is missing from ChatMessagesManager GameObject.");
			}

			Instance = this;
		}

		private IEnumerator QueueChatMessageHistory(ChatMessageHistory chatMessageHistory)
		{
			WaitForSecondsRealtime waitForSeconds = new(1f);
			while(chatMessageHistory.InUse)
			{
				Debug.Log("ChatMessageHistory {" + chatMessageHistory.name + "} is currently in use, waiting...");
				yield return waitForSeconds;
			}
			chatMessageHistory.InUse = true;

			yield return StartCoroutine(ShowChatMessageHistory(chatMessageHistory));

			chatMessageHistory.InUse = false;

		}

		private IEnumerator ShowChatMessageHistory(ChatMessageHistory chatMessageHistory)
		{
			WaitForSecondsRealtime waitForSeconds = new(1f);

			Debug.Log("Starting to show chat message history:" + chatMessageHistory.name);

			switch(chatMessageHistory.MessageHistoryOrder)
			{
				case ChatMessageHistoryOrder.Chronological:
					chatMessageHistory.ResetChatHistory();
					chatMessageHistory.ResetAmountFired();
					break;
				case ChatMessageHistoryOrder.Achronological:
					chatMessageHistory.ResetAmountFired();
					break;
				default:
					throw new System.Exception("Invalid ChatMessageHistoryType.");
			}

			ChatMessageData messageData;

			int recursionSafety = 128; // prevent infinite loops
			Debug.Log("ShowChatMessageHistory | Entering message display loop.");
			while (!chatMessageHistory.FiredOut && recursionSafety-- > 0)
			{
				if(PauseMenu.Instance.IsPaused)
				{
					yield return waitForSeconds;
					continue;
				}

				// Debug.Log(indentation + "Getting next message from chat message history.");
				messageData = chatMessageHistory.GetNextMessage();
				if (messageData == null)
				{
					Debug.LogWarning("ShowChatMessageHistory | Retrieved null ChatMessageData from ChatMessageHistory: " + chatMessageHistory.name);
					continue;
				}
				float delayTillNext = messageData.DelayTillNext;
				float displayDuration = messageData.DisplayDuration;
				// Debug.Log(indentation + output + "Displaying message with delay: " + delayTillNext + " and duration: " + displayDuration);
				StartCoroutine(ShowMessageAnimation(messageData, displayDuration));
				if(activeMessages.Count >= swipeTreshold)
				{
					StartCoroutine(SwipeCascade());
				}
				yield return new WaitForSecondsRealtime(delayTillNext);
			}

			switch(chatMessageHistory.MessageHistoryOrder)
			{
				case ChatMessageHistoryOrder.Chronological:
					chatMessageHistory.ResetChatHistory();
					chatMessageHistory.ResetAmountFired();
					break;
				case ChatMessageHistoryOrder.Achronological:
					chatMessageHistory.ResetAmountFired();
					break;
				default:
					throw new System.Exception("Invalid ChatMessageHistoryType.");
			}

			StartCoroutine(SwipeCascade());

			// Debug.Log("Finished showing chat message history:" + chatMessageHistory.name);
		}

		private IEnumerator SwipeCascade()
        {
			// Debug.Log("Starting swipe cascade.");
			WaitForSecondsRealtime waitForSeconds = new(0.2f);
            for (int i = 0; i < activeMessages.Count; i++)
			{
				if(PauseMenu.Instance.IsPaused)
				{
					yield return waitForSeconds;
					continue;
				}

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
			WaitForSecondsRealtime waitForSeconds = new(1f);

			if (messageData == null)
			{
				Debug.LogWarning("ShowMessageAnimation | Attempted to show a null ChatMessageData.");
				yield break;
			}
			// Debug.Log("ShowMessageAnimation | Showing message animation for message from: " + messageData.GetSenderName);

			float typingPreview = messageData.TypingPreview;
			Vector3 startScale = new(0.5f, 0.3f, 1f);

			ChatMessage messageScript = Instantiate(messagePrefab, messageParent as RectTransform);

			Sprite senderImage = messageImageSelector.GetSenderImage(messageData.GetSenderName);
			Sprite senderBackgroundImage = messageImageSelector.GetSenderBackgroundImage(messageData.GetSenderName);

			messageScript.InitializeMessage(messageData, senderImage, senderBackgroundImage);
			activeMessages.Add(messageScript);
			// Debug.Log("ShowMessageAnimation | Starting animation for message: " + messageScript.name);

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
				if(PauseMenu.Instance.IsPaused)
				{
					yield return waitForSeconds;
					continue;
				}

				typingPreview -= Time.unscaledDeltaTime;
				if (messageScript == null || messageScript.gameObject == null)
				{
					yield break;
				}
				yield return null;
			}

			float elapsedTime = 0f;
			// Debug.Log("ShowMessageAnimation | Message instantiated: " + messageScript.name);
			messageScript.SetSenderText(messageData.GetSenderName);
			messageScript.SetMessageText(messageData.MessageContent);

			messageScript.Written = true;
			// Here you can add animation code if needed
			while (elapsedTime < popUpDuration)
			{
				if(PauseMenu.Instance.IsPaused)
				{
					yield return waitForSeconds;
					continue;
				}

				elapsedTime += Time.unscaledDeltaTime;
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
				if(PauseMenu.Instance.IsPaused)
				{
					yield return waitForSeconds;
					continue;
				}

				elapsedTime += Time.unscaledDeltaTime;
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
			WaitForSecondsRealtime waitForSeconds = new(1f);
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
				if(PauseMenu.Instance.IsPaused)
				{
					yield return waitForSeconds;
					continue;
				}

				elapsedTime += Time.unscaledDeltaTime;
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