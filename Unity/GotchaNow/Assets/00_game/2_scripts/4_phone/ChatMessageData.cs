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
			[InspectorName("TurtleHerder")] TurtleHerder,
			[InspectorName("Ikigai Tournament")] IkigaiTournament = 10
        }
		
		[Header("Start")]
		[SerializeField] private float typingPreview = 0f;

		[Header("Message Content")]
		[SerializeField] private SenderName senderName = SenderName.KingBob;
		[SerializeField] [TextArea(3, 10)] private string messageContent = "";

		[Header("End")]
		[SerializeField] private float delayTillNext = 0f;
		[SerializeField] private float messageLifetime = 0f;

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
					case SenderName.IkigaiTournament:
						return "Ikigai Tournament";
					default:
						throw new ArgumentOutOfRangeException();
				}
            }
        }
		
		public string MessageContent => messageContent;

		public float TypingPreview => typingPreview;
		public float DelayTillNext => delayTillNext;
		public float DisplayDuration => messageLifetime;
	}
}
