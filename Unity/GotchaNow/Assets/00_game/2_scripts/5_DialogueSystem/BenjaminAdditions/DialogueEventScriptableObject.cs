using UnityEngine;
using UnityEngine.Events;
namespace GotchaNow
{
	public class DialogueEventScriptableObject : ScriptableObject
	{
		[field: SerializeField] public string EventName { get; private set; }
        [field: SerializeField] public UnityEvent OnDialogueEvent { get; private set; }
	}
}
