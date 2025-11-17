using UnityEngine;
using System;

namespace GotchaNow
{
	[Serializable]
	public class ChatMessageData
	{
		[Header("Start")]
		[SerializeField] private float typingPreview = 2f;

		[Header("Message Content")]
		[SerializeField] private string senderName;
		[SerializeField] private Sprite senderAvatar;
		[SerializeField] [TextArea(3, 10)] private string messageContent;

		[Header("End")]
		[SerializeField] private float delayTillNext = 3f;
		[SerializeField] private float messageLifetime = 5f;

		public string SenderName => senderName;
		public Sprite SenderAvatar => senderAvatar;
		public string MessageContent => messageContent;

		public float TypingPreview => typingPreview;
		public float DelayTillNext => delayTillNext;
		public float DisplayDuration => messageLifetime;
	}
}
