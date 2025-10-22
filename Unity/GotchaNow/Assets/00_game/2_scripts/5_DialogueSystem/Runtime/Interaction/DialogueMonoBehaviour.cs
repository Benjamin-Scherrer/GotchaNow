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
        [SerializeField] protected DialogueContainer narrativeScriptableObject;
        [SerializeField] protected NarrativeController narrativeController;
        // [SerializeField] protected DialogueEvent[] dialogueEvents;

        // //My Additions
        // [SerializeField] protected DialogueEventScriptableObject[] dialogueEventScriptableObjects;


        // protected virtual void Awake()
        // {
        //     // Debug.Log("Awake called in DialogueMonoBehaviour");
        //     dialogueEvents = GetDialogueEvents;
        // }
        
       

        //End My Additions

        /// <summary>
        /// Start the dialogue using the narrative controller and by loading the narrative scriptable object.
        /// </summary>
        // protected void StartDialogue() => narrativeController.BeginNarration(narrativeScriptableObject, dialogueEvents);
        protected void StartDialogue()
        {

            // narrativeController.BeginNarration(narrativeScriptableObject, dialogueEvents);
            narrativeController.BeginNarration(narrativeScriptableObject);
        }
    }
}