using DialogueSystem.Data;
using DialogueSystem.Runtime.Narration;
using GotchaNow;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DialogueSystem.Runtime.Interaction
{
    public abstract class DialogueMonoBehaviour : MonoBehaviour
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
        
        [SerializeField] protected DialogueContainer narrativeScriptableObject;
        [SerializeField] protected NarrativeController narrativeController;
        // [SerializeField] protected KeyCode skipInput = KeyCode.Space;
        [SerializeField] protected DialogueEvent[] dialogueEvents;

        //My Additions
        [SerializeField] protected DialogueEventScriptableObject[] dialogueEventScriptableObjects;

        protected virtual void Awake()
        {
            Debug.Log("Awake called in DialogueMonoBehaviour");
            dialogueEvents = GetDialogueEvents;
        }
        private DialogueEvent[] GetDialogueEvents
        {
            get
            {
                int dialogueEventLength = dialogueEvents != null ? dialogueEvents.Length : 0;
                int dialogueEventScriptableObjectLength = dialogueEventScriptableObjects != null ? dialogueEventScriptableObjects.Length : 0;

                DialogueEvent[] combinedEvents = new DialogueEvent[dialogueEventLength + dialogueEventScriptableObjectLength];
                dialogueEvents.CopyTo(combinedEvents, 0);

                for (int i = 0; i < dialogueEventScriptableObjectLength; i++)
                {
                    DialogueEventScriptableObject dialogueEventScriptableObject = dialogueEventScriptableObjects[i];
                    if (dialogueEventScriptableObjects != null)
                    {
                        combinedEvents[dialogueEvents.Length + i] = new DialogueEvent(dialogueEventScriptableObject.EventName, dialogueEventScriptableObject.OnDialogueEvent);
                    }
                }
                Debug.Log("Total Dialogue Events Combined: " + combinedEvents.Length);
                return combinedEvents;
            }
        }

        //End My Additions

        [SerializeField] protected InputActionReference skipAction;
        
        /// <summary>
        /// Skips the current dialogue text if the player presses the skip input.
        /// </summary>
        protected void SkipDialogueWithInput()
        {
            // Requires UnityEngine.InputSystem package
            if (!skipAction.action.WasPressedThisFrame() || narrativeController.IsChoosing || !narrativeController.IsNarrating)
            {
            return;
            }

            narrativeController.NextNarrative();
        }

        /// <summary>
        /// Start the dialogue using the narrative controller and by loading the narrative scriptable object.
        /// </summary>
        // protected void StartDialogue() => narrativeController.BeginNarration(narrativeScriptableObject, dialogueEvents);
        protected void StartDialogue() => narrativeController.BeginNarration(narrativeScriptableObject, dialogueEvents);
    }
}