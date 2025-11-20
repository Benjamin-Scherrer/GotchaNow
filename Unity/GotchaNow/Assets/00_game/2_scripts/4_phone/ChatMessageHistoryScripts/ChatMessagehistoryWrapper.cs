using System;
using UnityEngine;

namespace GotchaNow
{
    [Serializable]
    public class ChatMessageHistoryWrapper
    {
        [SerializeField] private ChatMessageHistory[] conglomerateHistories;
        // Properties
        public ChatMessageHistory[] ConglomerateHistories => conglomerateHistories;
    }
}