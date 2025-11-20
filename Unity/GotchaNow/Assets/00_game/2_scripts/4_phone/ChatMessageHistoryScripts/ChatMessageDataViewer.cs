using System;
using UnityEngine;
using UnityEssentials;

namespace GotchaNow
{
    [Serializable]
    public class ChatMessageDataViewer
    {
        [SerializeField] private FireMode fireMode = FireMode.fireAll;
        [ShowIf("fireMode", FireMode.fireAmount)]
        [SerializeField] private int amountToFire;
        [SerializeField] private ChatMessageData[] chatMessages;

        // Properties
        public ChatMessageData[] ChatMessages => chatMessages;
        public int AmountToFire
        {
            get
            {
                if (fireMode == FireMode.fireAll)
                {
                    return chatMessages.Length;
                }
                else
                {
                    return amountToFire;
                }
            }
        }
        public FireMode GetFireMode { get { return fireMode; } }
    }
}