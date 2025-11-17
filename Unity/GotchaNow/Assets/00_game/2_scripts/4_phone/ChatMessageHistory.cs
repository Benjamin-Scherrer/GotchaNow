using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials;

namespace GotchaNow
{
	public class ChatMessageHistory : ScriptableObject
	{
		[Serializable]
		public class ChatMessageDataViewer {
			[SerializeField] private ChatMessageData[] chatMessages;
			public ChatMessageData[] ChatMessages => chatMessages;
			// public ChatMessageData[] ChatMessages => chatMessages;
		}

		[Serializable]
		public class ChatMessageHistoryWrapper
        {
            [SerializeField] private ChatMessageHistory[] conglomerateHistories;
			public ChatMessageHistory[] ConglomerateHistories => conglomerateHistories;
        }

		[Header("Chat Message History Type")]
		[SerializeField] private ChatMessageDataType messageDataType = ChatMessageDataType.Chronological;

		[HideIf("messageDataType", ChatMessageDataType.Conglomerate)]
		 [SerializeField] private ChatMessageDataViewer chatMessageDataViewer;
		// [SerializeField] private ChatMessageData[] chatMessages;
		// [SerializeField] private bool test1;

		[ShowIf("messageDataType", ChatMessageDataType.Conglomerate)]
		[SerializeField] private ChatMessageHistoryWrapper chatMessageHistoryWrapper;
		
		// [SerializeField] private bool test2;

		private ChatMessageData[] chatMessages => chatMessageDataViewer.ChatMessages;
		private ChatMessageHistory[] conglomerateHistories => chatMessageHistoryWrapper.ConglomerateHistories;
		//for chronological type
		private int currentIndex = 0;

		//for achronological type
		private List<int> achronologicalIndices;

		// for conglomerate type
		private int conglomerateCurrentHistoryIndex = 0;
		
		//PUBLIC
		public ChatMessageData GetNextMessage()
		{
            return messageDataType switch
            {
                ChatMessageDataType.Chronological => GetNextMessageChronological(),
                ChatMessageDataType.Achronological => GetNextMessageAchronological(),
                ChatMessageDataType.Conglomerate => GetNextMessageConglomerate(),
                _ => throw new Exception("Invalid ChatMessageDataType."),
            };
        }

		public void ResetChatHistory()
		{
			switch(messageDataType)
			{
				case ChatMessageDataType.Chronological:
					ResetChatHistoryChronological();
					break;
				case ChatMessageDataType.Achronological:
					ResetChatHistoryAchronological();
					break;
				case ChatMessageDataType.Conglomerate:
					ResetChatHistoryConglomerate();
					break;
				default:
					throw new Exception("Invalid ChatMessageDataType.");
			}
		}

		// PRIVATE
		private ChatMessageData GetNextMessageChronological()
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

		private ChatMessageData GetNextMessageAchronological()
        {
            if (chatMessages == null || chatMessages.Length == 0)
            {
				Debug.LogWarning("ChatMessageHistory has no messages.");
				return null;
            }
			if (achronologicalIndices == null)
            {
                Debug.LogWarning("Achronological indices not initialized. Resetting chat history.");
            }
			if(achronologicalIndices.Count == 0)
			{
				Debug.Log("Reached the end of ChatMessageHistory.");
				return null;
			}

			int nextIndex = achronologicalIndices.PickRandom();
			achronologicalIndices.Remove(nextIndex);
			return chatMessages[nextIndex];
		}

		private ChatMessageData GetNextMessageConglomerate()
		{
			if(conglomerateHistories == null || conglomerateHistories.Length == 0)
			{
				Debug.LogWarning("Conglomerate ChatMessageHistory has no histories.");
				return null;
			}
			if(conglomerateCurrentHistoryIndex >= conglomerateHistories.Length)
			{
				Debug.Log("Reached the end of Conglomerate ChatMessageHistory.");
				return null;
			}

			ChatMessageData message = null;
			for(int i = 0; i < conglomerateHistories.Length; i++)
            {
				if(conglomerateCurrentHistoryIndex >= conglomerateHistories.Length)
				{
					break;
				}
				ChatMessageHistory currentHistory = conglomerateHistories[conglomerateCurrentHistoryIndex];
				if(currentHistory == null)
				{
					Debug.LogWarning("A ChatMessageHistory in the Conglomerate is null. Skipping.");
					conglomerateCurrentHistoryIndex++;
					continue;
				}
				if(currentHistory == this)
				{
					Debug.LogWarning("A ChatMessageHistory in the Conglomerate is itself. Skipping to avoid infinite loop.");
					conglomerateCurrentHistoryIndex++;
					continue;
				}
				message = currentHistory.GetNextMessage();
				if(message == null)
                {
					conglomerateCurrentHistoryIndex++;
                    continue;
                }
				break;
            }
			if(message == null)
			{
				Debug.Log("Reached the end of Conglomerate ChatMessageHistory.");
				return null;
			}
			return message;
		}

		//RESET
		private void ResetChatHistoryChronological()
		{
			currentIndex = 0;
		}

		private void ResetChatHistoryAchronological()
		{
			achronologicalIndices = new List<int>();
			for (int i = 0; i < chatMessages.Length; i++)
			{
				achronologicalIndices.Add(i);
			}
			//shuffle the indices
			for (int i = 0; i < achronologicalIndices.Count; i++)
			{
				int randomIndex = UnityEngine.Random.Range(0, achronologicalIndices.Count);
                (achronologicalIndices[randomIndex], achronologicalIndices[i]) 
					= (achronologicalIndices[i], achronologicalIndices[randomIndex]);
            }
        }

		private void ResetChatHistoryConglomerate()
		{
			conglomerateCurrentHistoryIndex = 0;
			foreach(var history in conglomerateHistories)
			{
				history.ResetChatHistory();
			}
		}
	}
}
