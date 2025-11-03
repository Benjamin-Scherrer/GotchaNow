using GotchaNow;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using DialogueSystem.Runtime.Narration;

namespace DialogueSystem.Runtime.Interaction
{
    public class Interactor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NarrativeController narrativeController;

        [Header("Interaction Settings")]
        //To interact in 3D space.
        [SerializeField] private float interactionDistance;

        [Header("Input Actions")]
        //Just general click action
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private InputActionReference skipAction;


        private List<GameObject> hintObjects = new List<GameObject>();

        //PRIVATE METHODS

        private void Update()
        {
            //Order is important.
            //1
            if (SkipDialogueWithInput()) return;

            //2
            if (InteractWithCharacter()) return;
        }

        private bool InteractWithCharacter()
        {
            if (interactAction == null || interactAction.action == null)
            {
                throw new System.Exception("Interact Action is not assigned.");
            }
            //Try get the interactable in range.
            InteractableDialogue interactable = InteracteeManager.Instance.GetInteractableDialogue(transform.position, interactionDistance);
            if(hintObjects.Count > 0)
            {
                hintObjects.ForEach(hint => hint.SetActive(false));
                hintObjects.Clear();
            }
            if (interactable == null) return false;
            Debug.Log("Found Interactable");
            if (!interactable.CanInteract) return false;
            Debug.Log("Can Interact");
            hintObjects.Add(interactable.HintObject);
            hintObjects.ForEach(hint => hint.SetActive(true));
            Debug.Log("Showing hint for " + interactable.name);
            if (!interactAction.action.WasPressedThisFrame()) return false;
            Debug.Log("Interact Pressed");
            Debug.Log("Interacting with " + interactable.name);
            interactable.Interact();
            return true;
        }


        /// <summary>
        /// Skips the current dialogue text if the player presses the skip input.
        /// </summary>
        protected bool SkipDialogueWithInput()
        {
            if (!skipAction.action.WasPressedThisFrame()) return false;
            // Debug.Log("SkipDialogueWithInput called from: " + gameObject.name);
            // if (DialogueStartedRightNow)
            // {
            //     DialogueStartedRightNow = false;
            //     return;
            // }

            if (narrativeController.IsChoosing) return false;
            if (!narrativeController.IsNarrating) return false;

            narrativeController.NextNarrative();

            return true;
        }

        public void PrepareInteraction()
        {
            gameObject.SetActive(true);
        }

        public void EndInteraction()
        {
            gameObject.SetActive(false);
        }
    }
}