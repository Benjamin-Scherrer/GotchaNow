using UnityEngine;

namespace DialogueSystem.Runtime.Interaction
{
    public class InteractableDialogue : DialogueMonoBehaviour
    {
        [SerializeField] private bool stopInteractAtNarrativeEnd;
        
        public bool CanInteract =>
            !((stopInteractAtNarrativeEnd && (narrativeScriptableObject is { IsNarrativeEndReached: true })) ||
              narrativeController.IsNarrating);

        private void Update() => SkipDialogueWithInput();

        public void Interact() => StartDialogue();
    }
}