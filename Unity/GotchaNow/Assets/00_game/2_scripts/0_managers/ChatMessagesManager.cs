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
		private ChatMessageSelector messageSelector;
		[SerializeField] private List<ChatMessage> activeMessages = new();

		//PUBLIC
		public void DisplayMessageHistory()
		{
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
		}

		private void Start()
		{
			// Example usage
			// SpawnMessageHistory(messageSelector.GetChatMessageHistory());

			StartCoroutine(LoopSpawnMessages());
		}

		private IEnumerator LoopSpawnMessages()
		{
            while (true)
            {
				yield return StartCoroutine(ShowChatMessageHistory(messageSelector.GetChatMessageHistory()));	
            }
		}

		private void SpawnMessageHistory(ChatMessageHistory chatMessageHistory)
		{
			if (chatMessageHistory == null)
			{
				Debug.LogWarning("Attempted to spawn a null ChatMessageHistory.");
				return;
			}
			StartCoroutine(ShowChatMessageHistory(chatMessageHistory));
		}

		private IEnumerator ShowChatMessageHistory(ChatMessageHistory chatMessageHistory)
		{
			yield return new WaitForSeconds(1f); // initial delay before starting chat message history
			chatMessageHistory.ResetChatHistory();
			ChatMessageData messageData;
			while ((messageData = chatMessageHistory.GetNextMessage()) != null)
			{
				float delayTillNext = messageData.DelayTillNext;
				StartCoroutine(ShowMessageAnimation(messageData));
				yield return new WaitForSeconds(delayTillNext);
			}

			// clean up after all messages have been shown
			chatMessageHistory.ResetChatHistory();
			foreach (var msg in activeMessages)
			{
				if (msg != null)
				{
					Destroy(msg.gameObject);
				}
			}
			activeMessages.Clear();
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
			messageScript.SetSenderText(messageData.SenderName + " is typing...");
			messageScript.SetMessageText("");
			PushDown();
			while (typingPreview > 0f)
			{
				typingPreview -= Time.deltaTime;
				yield return null;
			}

			float elapsedTime = 0f;
			Debug.Log("Message instantiated: " + messageScript.name);
			messageScript.SetSenderText(messageData.SenderName);
			messageScript.SetMessageText(messageData.MessageContent);
			// Here you can add animation code if needed
			while (elapsedTime < popUpDuration)
			{
				elapsedTime += Time.deltaTime;
				float timeCoefficient = elapsedTime / popUpDuration;

				Vector3 scaleValue = Vector3.Lerp(startScale, Vector3.one, timeCoefficient);
				messageScript.ScaleMessageRespectingFontSize(scaleValue);
				PushDown();
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