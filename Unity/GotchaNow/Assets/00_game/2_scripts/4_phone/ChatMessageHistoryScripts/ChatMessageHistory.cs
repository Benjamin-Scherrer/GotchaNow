using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials;

namespace GotchaNow
{
	public class ChatMessageHistory : ScriptableObject
	{
		// FIELDS
		[Header("Chat Message History Type")]
		[SerializeField] private ChatMessageHistoryType chatMessageHistoryType = ChatMessageHistoryType.Simple;
		[SerializeField] private ChatMessageHistoryOrder messageHistoryOrder = ChatMessageHistoryOrder.Chronological;

		[ShowIf("chatMessageHistoryType", ChatMessageHistoryType.Simple)]
		[SerializeField] private ChatMessageDataViewer chatMessageDataViewer;
		// [SerializeField] private ChatMessageData[] chatMessages;
		// [SerializeField] private bool test1;

		[ShowIf("chatMessageHistoryType", ChatMessageHistoryType.Conglomerate)]
		[SerializeField] private ChatMessageHistoryWrapper chatMessageHistoryWrapper;
		
		private bool inUse = false;
		// PROPERTIES
		private ChatMessageData[] chatMessages => chatMessageDataViewer.ChatMessages;
		private ChatMessageHistory[] conglomerateHistories => chatMessageHistoryWrapper.ConglomerateHistories;
		//for chronological type
		private int currentIndex = 0;

		//for achronological type
		private List<int> achronologicalIndices;

		// for conglomerate type
		private int conglomerateCurrentHistoryIndex = 0;

		// amount to fire
		private int amountFired = 0;
		public bool FiredOut
        {
            get
            {
				switch(chatMessageHistoryType)
				{
					case ChatMessageHistoryType.Simple:
                        int amountToFire = chatMessageDataViewer.GetFireMode switch
                        {
                            FireMode.fireAll => chatMessages.Length,
                            FireMode.fireAmount => chatMessageDataViewer.AmountToFire,
                            _ => throw new Exception("Invalid FireMode."),
                        };
						if(amountToFire > chatMessages.Length 
							&& messageHistoryOrder == ChatMessageHistoryOrder.Chronological)
							throw new Exception("Amount to fire exceeds available chat messages in Chronological mode.");

                        // Debug.Log($"Simple FiredOut check. AmountFired: {amountFired}, AmountToFire: {amountToFire}");
						// Debug.Log("Simple FiredOut check: " + (amountFired >= amountToFire));
						return amountFired >= amountToFire;

					case ChatMessageHistoryType.Conglomerate:
						bool allFiredOut = true;
						foreach(var history in conglomerateHistories)
						{
							if(history.FiredOut) continue;
							allFiredOut = false;
							break;
						}
						// Debug.Log("Conglomerate FiredOut check: " + allFiredOut);
						return allFiredOut;
					default:
						throw new Exception("Invalid ChatMessageDataType.");
				}
            }
        }

		public bool InUse 
		{ 
			get 
			{ 
				switch(chatMessageHistoryType)
				{
					case ChatMessageHistoryType.Simple:
						return inUse;
					case ChatMessageHistoryType.Conglomerate:
						if (inUse) return true;
						foreach (var history in conglomerateHistories)
						{
							if (history.InUse) return true;
						}
						return false;
					default:
						throw new Exception("Invalid ChatMessageDataType.");
				}
			} 
			set 
			{ 
				switch(chatMessageHistoryType)
				{
					case ChatMessageHistoryType.Simple:
						inUse = value;
						break;
					case ChatMessageHistoryType.Conglomerate:
						inUse = value;
						foreach (var history in conglomerateHistories)
						{
							history.InUse = value;
						}
						break;
					default:
						throw new Exception("Invalid ChatMessageDataType.");
				}
			} 
		}

		// METHODS
		public void ResetAmountFired()
		{
			switch(chatMessageHistoryType)
			{
				case ChatMessageHistoryType.Simple:
					// Debug.Log("Resetting amount fired for Simple ChatMessageHistory.");
					amountFired = 0;
					break;
				case ChatMessageHistoryType.Conglomerate:
					// Debug.Log("Resetting amount fired for Conglomerate ChatMessageHistory.");
					foreach(var history in conglomerateHistories)
					{
						history.ResetAmountFired();
					}
					break;
				default:
					throw new Exception("Invalid ChatMessageDataType.");
			}
		}

		public ChatMessageHistoryType ChatMessageHistoryType => chatMessageHistoryType;
		public ChatMessageHistoryOrder MessageHistoryOrder => messageHistoryOrder;

		//PUBLIC
		public ChatMessageData GetNextMessage()
		{
			amountFired++;
			ChatMessageData chatMessageData = chatMessageHistoryType switch
            {
                ChatMessageHistoryType.Simple => GetNextMessageSimple(),
                ChatMessageHistoryType.Conglomerate => GetNextMessageConglomerate(),
                _ => throw new Exception("Invalid ChatMessageDataType."),
            } ?? throw new Exception("ChatMessageData is null.");
            return chatMessageData;
        }

		public void ResetAllOnAwake()
		{
			ResetChatHistory();
			ResetAmountFired();
			inUse = false;
		}

		public void ResetChatHistory()
		{
			switch(chatMessageHistoryType)
			{
				case ChatMessageHistoryType.Simple:
					ResetChatHistorySimple();
					break;
				case ChatMessageHistoryType.Conglomerate:
					ResetChatHistoryConglomerate();
					break;
				default:
					throw new Exception("Invalid ChatMessageDataType.");
			}
		}

		// PRIVATE
		private ChatMessageData GetNextMessageSimple()
		{
			ChatMessageData chatMessageData = messageHistoryOrder switch
            {
                ChatMessageHistoryOrder.Chronological => GetNextMessageSimpleChronological(),
                ChatMessageHistoryOrder.Achronological => GetNextMessageSimpleAchronological(),
                _ => throw new Exception("Invalid ChatMessageHistoryOrder."),
            } ?? throw new Exception("ChatMessageData is null.");
			return chatMessageData;
        }

		private ChatMessageData GetNextMessageSimpleChronological()
        {
            if (chatMessages == null || chatMessages.Length == 0)
			{
				throw new Exception("SinmpleChronological | ChatMessageHistory has no messages.");
			}

			if (currentIndex >= chatMessages.Length)
			{
				switch (chatMessageDataViewer.GetFireMode)
				{
					case FireMode.fireAll:
						// There is a big change you forgot to set the firemode to random
						throw new Exception("SimpleChronological | Reached the end of ChatMessageHistory." + 
						" CurrentIndex: " + currentIndex + ", ChatMessages Length: " + chatMessages.Length);
					case FireMode.fireAmount:
						Debug.LogWarning("SimpleChronological | Reached the end of ChatMessageHistory with fireAmount mode." +
							"You should probably make sure the amount to fire matches the number of messages availabley");
						return new ChatMessageData(); // return a blank message to indicate the end
				}
			}

			ChatMessageData message = chatMessages[currentIndex]
				?? throw new Exception("SimpleChronological | ChatMessageData at index " + currentIndex + " is null.");
            currentIndex++;
			return message;
        }

		private ChatMessageData GetNextMessageSimpleAchronological()
        {
            if (chatMessages == null || chatMessages.Length == 0)
            {
				throw new Exception("SimpleAchronological | ChatMessageHistory has no messages.");
            }
			if (achronologicalIndices == null)
            {
                throw new Exception("SimpleAchronological | Achronological indices not initialized. Resetting chat history.");
			}
			// if(achronologicalIndices.Count == 0)
			// {
			// 	Debug.Log("SimpleAchronological | Reached the end of ChatMessageHistory.");
			// 	return null;
			// }

			int recursionSafety = 128;
			while(recursionSafety-- > 0)
			{
				if(achronologicalIndices.Count == 0)
				{
					ResetChatHistorySimpleAchronological();
				}
				int nextIndex = achronologicalIndices.PickRandom();
				achronologicalIndices.Remove(nextIndex);
				if(nextIndex < 0 || nextIndex >= chatMessages.Length)
                {
					throw new Exception("Achronological index out of bounds.");
                }
				ChatMessageData message = chatMessages[nextIndex]
					?? throw new Exception("SimpleAchronological | ChatMessageData at index " + nextIndex + " is null.");
				return message;
			}
			throw new Exception("Failed to get next achronological message after multiple attempts.");
		}

		// CONGLOMERATE
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

            ChatMessageData chatMessageData = messageHistoryOrder switch
            {
                ChatMessageHistoryOrder.Chronological => GetNextMessageConglomerateChronological(),
                ChatMessageHistoryOrder.Achronological => GetNextMessageConglomerateAchronological(),
                _ => throw new Exception("Invalid ChatMessageHistoryOrder."),
            } ?? throw new Exception("ChatMessageData is null.");
			return chatMessageData;
        }

		private ChatMessageData GetNextMessageConglomerateChronological()
		{
			ChatMessageData message = null;
			int repetitions = 128;
			while(repetitions-- > 0)
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
				if(currentHistory.FiredOut)
				{
					conglomerateCurrentHistoryIndex++;
					continue;
				}
				message = currentHistory.GetNextMessage() 
					?? throw new Exception("ChatMessageData is null.");
				break;
            }
			if(message == null)
			{
				Debug.Log("Reached the end of Conglomerate ChatMessageHistory.");
				return null;
			}
			return message;
		}

		private ChatMessageData GetNextMessageConglomerateAchronological()
		{
			ChatMessageData message = null;
			conglomerateCurrentHistoryIndex = UnityEngine.Random.Range(0, conglomerateHistories.Length);
			int repetitions = 100;
			while (repetitions-- > 0)
            {
				if(conglomerateCurrentHistoryIndex >= conglomerateHistories.Length)
				{
					conglomerateCurrentHistoryIndex = 0;
					continue;
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
				if(currentHistory.FiredOut)
				{
					conglomerateCurrentHistoryIndex++;
					continue;
				}
				message = currentHistory.GetNextMessage() 
					?? throw new Exception("ChatMessageData is null.");
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
		private void ResetChatHistorySimple()
		{
			switch (messageHistoryOrder)
			{
				case ChatMessageHistoryOrder.Chronological:
					ResetChatHistorySimpleChronological();
					break;
				case ChatMessageHistoryOrder.Achronological:
					ResetChatHistorySimpleAchronological();
					break;
				default:
					throw new Exception("Invalid ChatMessageHistoryOrder.");
			}
		}
		private void ResetChatHistorySimpleChronological()
		{
			currentIndex = 0;
		}

		private void ResetChatHistorySimpleAchronological()
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
