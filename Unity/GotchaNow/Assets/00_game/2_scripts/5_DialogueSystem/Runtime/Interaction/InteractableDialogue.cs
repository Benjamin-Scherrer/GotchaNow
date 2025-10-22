using UnityEngine;
using DialogueSystem.Runtime.Interaction;
using UnityEngine.InputSystem;

namespace GotchaNow
{
    [RequireComponent(typeof(DialogueSelector))]
    public class InteractableDialogue : DialogueMonoBehaviour
    {
        private bool stopInteractAtNarrativeEnd;
        private DialogueSelector dialogueSelector;

        // public bool CanInteract =>
        //     !((stopInteractAtNarrativeEnd && (narrativeScriptableObject is { IsNarrativeEndReached: true })) ||
        //       narrativeController.IsNarrating);

        public bool CanInteract
        {
            get
            {
                if (narrativeScriptableObject == null)
                {
                    return false;
                }
                
                // If we've been configured to stop interactions after the narrative ends,
                // and we have a narrative that has reached the end, then disallow interaction.
                bool isStoppedBecauseNarrativeEnded =
                    stopInteractAtNarrativeEnd &&
                    (narrativeScriptableObject is { IsNarrativeEndReached: true });

                // If the narrative controller is currently narrating, disallow interaction.
                bool isNarratingNow = narrativeController.IsNarrating;

                // We can interact only when neither blocking condition is true.
                return !(isStoppedBecauseNarrativeEnded || isNarratingNow);
            }
        }

        // private void Update() => SkipDialogueWithInput();

        public void PrepareInteraction()
        {
            narrativeScriptableObject = dialogueSelector.GetNarrativeScriptableObject();
        }
        
        public void Interact()
        {
            if (narrativeScriptableObject == null)
            {
                throw new System.Exception("No narrative scriptable object found for the current interaction.");
            }

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
        
        private void Start()
        {
            InteracteeManager.Instance.RegisterInteractable(this);
        }
    }
}