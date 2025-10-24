using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem.Runtime.Interaction
{
	[System.Serializable]
	public class DialogueEvent
	{
		public DialogueEvent(string eventName, UnityEvent onDialogueEvent)
		{
			this.EventName = eventName;
			this.OnDialogueEvent = onDialogueEvent;
		}

		[field: SerializeField] public string EventName { get; private set; }
		[field: SerializeField] public UnityEvent OnDialogueEvent { get; private set; }

		public void InvokeEvent() => OnDialogueEvent?.Invoke();

	}
}
