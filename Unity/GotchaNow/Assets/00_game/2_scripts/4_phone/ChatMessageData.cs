using UnityEngine;
using System;

namespace GotchaNow
{
	[Serializable]
	public class ChatMessageData
	{
		[Header("Message Content")]
		[SerializeField] private string senderName;
		[SerializeField] private Sprite senderAvatar;
		[SerializeField] [TextArea(3, 10)] private string messageContent;

		[Header("Display Settings")]
		[SerializeField] private float typingPreview = 2f;
		[SerializeField] private float delayTillNext = 3f;

		public string SenderName => senderName;
		public Sprite SenderAvatar => senderAvatar;
		public string MessageContent => messageContent;

		public float TypingPreview => typingPreview;
		public float DelayTillNext => delayTillNext;
	}
}
