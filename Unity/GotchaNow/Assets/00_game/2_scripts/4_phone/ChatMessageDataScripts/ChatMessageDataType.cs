using System;


namespace GotchaNow
{
    [Serializable]
    public enum ChatMessageHistoryType
    {
        Simple,
        Conglomerate
    }

    public enum ChatMessageHistoryOrder
    {
        Chronological,
        Achronological
    }
}