using DialogueSystem.Scene;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DialogueSystem.Runtime.Interaction
{
    public class Interactor : MonoBehaviour
    {
        //To interact in 3D space.
        [SerializeField] private Transform interactSource;
        [SerializeField] private float interactionDistance;

        //Just general click action
        [SerializeField] private InputActionReference interactAction;
    
        private void Update()
        {
            InteractWithCharacter();
        }

        private void InteractWithCharacter()
        {

            if (interactAction == null || interactAction.action == null)
            {
                throw new System.Exception("Interact Action is not assigned.");
            }

            if (!interactAction.action.WasPressedThisFrame())
            return;
            // Debug.Log("Interact Pressed");
            Collider[] colliders = new Collider[10]; // Adjust size as needed
            int hitCount = Physics.OverlapSphereNonAlloc(interactSource.position, interactionDistance, colliders);
            InteractableDialogue interactable = null;
            for (int i = 0; i < hitCount; i++)
            {
                interactable = colliders[i].GetComponent<InteractableDialogue>();
                if (interactable != null)
                    break;
            }
            if (interactable == null) return;
            // Debug.Log("Found Interactable");
            if (!interactable.CanInteract) return;
            interactable.Interact();
        }  
    }
}