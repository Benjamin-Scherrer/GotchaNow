using UnityEngine;
using System;
using System.ComponentModel;

namespace GotchaNow
{
	[Serializable]
	public class ChatMessageData
	{
		[Serializable]
		private enum SenderName
        {
            [InspectorName("KingBob")] KingBob,
			[InspectorName("Honey._.Bear")] HoneyBear,
			[InspectorName("CeilingFanEnthusast")] CeilingFanEnthusast,
			[InspectorName("TurtleHerder")] TurtleHerder
        }
		
		[Header("Start")]
		[SerializeField] private float typingPreview = 2f;

		[Header("Message Content")]
		[SerializeField] private SenderName senderName;
		[SerializeField] private Sprite senderAvatar;
		[SerializeField] [TextArea(3, 10)] private string messageContent;

		[Header("End")]
		[SerializeField] private float delayTillNext = 3f;
		[SerializeField] private float messageLifetime = 5f;

		public string GetSenderName
        {
            get
            {
                switch (senderName)
				{
					case SenderName.KingBob:
						return "KingBob";
					case SenderName.HoneyBear:
						return "Honey._.Bear";
					case SenderName.CeilingFanEnthusast:
						return "CeilingFanEnthusast";
					case SenderName.TurtleHerder:
						return "TurtleHerder";
					default:
						throw new ArgumentOutOfRangeException();
				}
            }
        }
		public Sprite SenderAvatar => senderAvatar;
		public string MessageContent => messageContent;

		public float TypingPreview => typingPreview;
		public float DelayTillNext => delayTillNext;
		public float DisplayDuration => messageLifetime;
	}
}
