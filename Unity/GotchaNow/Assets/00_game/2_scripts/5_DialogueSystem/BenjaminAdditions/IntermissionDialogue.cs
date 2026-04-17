using UnityEngine;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Interaction;

namespace GotchaNow
{
	[RequireComponent(typeof(DialogueSelector))]
	public class IntermissionDialogue : DialogueMonoBehaviour
	{
		private DialogueSelector dialogueSelector;

		//PUBLIC
		public void Interact()
		{
			narrativeScriptableObject = dialogueSelector.GetNarrativeScriptableObject();

			if (narrativeScriptableObject == null)
			{
				// Debug.Log("No narrative scriptable object found for the current intermission.");
				return;
			}
			Debug.Log("Starting intermission dialogue: " + narrativeScriptableObject.name);
			StartDialogue();
		}

		//PRIVATE
		private void Awake()
		{
			if (TryGetComponent<DialogueSelector>(out dialogueSelector))
			{
			}
			else
			{
				throw new System.Exception("DialogueSelector component not found on the GameObject.");
			}
		}
		
		// private void Update() => SkipDialogueWithInput();
	}
}
