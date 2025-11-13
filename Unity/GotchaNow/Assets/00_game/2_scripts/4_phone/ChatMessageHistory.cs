using UnityEngine;

namespace GotchaNow
{
	public class ChatMessageHistory : ScriptableObject
	{
		[SerializeField] private ChatMessageData[] chatMessages;
		private int currentIndex = 0;
		
		//PUBLIC
		public ChatMessageData GetNextMessage()
		{
			if (chatMessages == null || chatMessages.Length == 0)
			{
				Debug.LogWarning("ChatMessageHistory has no messages.");
				return null;
			}

			if (currentIndex >= chatMessages.Length)
			{
				Debug.Log("Reached the end of ChatMessageHistory.");
				return null;
			}

			ChatMessageData message = chatMessages[currentIndex];
			currentIndex++;
			return message;
		}

		public void ResetChatHistory()
		{
			currentIndex = 0;
		}
	}
}
