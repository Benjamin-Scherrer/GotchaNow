using System.Collections.Generic;
using DialogueSystem.Runtime.Interaction;
using UnityEngine;

namespace GotchaNow
{
	[DefaultExecutionOrder(-100)]
	public class InteracteeManager : MonoBehaviour
	{
		public static InteracteeManager Instance { get; private set; }

		[Header("References")]
		[SerializeField] private Interactor interactor;

		[SerializeField] private List<InteractableDialogue> interactables = new();

		private void Awake()
		{
			if (Instance != null) throw new System.Exception("Multiple InteracteeManagers in scene.");
			Instance = this;
		}

		public void RegisterInteractable(InteractableDialogue interactable)
		{
			// Debug.Log("Trying to register interactable: " + interactable.name);
			if(interactables.Contains(interactable)) return; //throw new System.Exception("Interactable already registered.");
			// Debug.Log("Registering interactable: " + interactable.name);
			// Register the interactable
			interactables.Add(interactable);
		}

		public void UnregisterInteractable(InteractableDialogue interactable)
		{
			// Debug.Log("Trying to unregister interactable: " + interactable.name);
			if (!interactables.Contains(interactable)) return; //throw new System.Exception("Interactable not registered.");
			// Debug.Log("Unregistering interactable: " + interactable.name);
			// Unregister the interactable
			interactables.Remove(interactable);
		}

		public InteractableDialogue GetInteractableDialogue(Vector3 position, float maxDistance)
		{
			// Debug.Log("Searching for interactables. Count: " + interactables.Count);
			float closestDistance = float.MaxValue;
			InteractableDialogue closestInteractable = null;
			foreach (var interactable in interactables)
			{
				// Debug.Log("Checking interactable: " + interactable.name);
				if(!interactable.gameObject.activeInHierarchy) continue;
				float distance = Vector3.Distance(position, interactable.transform.position);
				if (distance <= maxDistance && distance < closestDistance)
				{
					closestDistance = distance;
					closestInteractable = interactable;
				}
			}
			return closestInteractable;
		}

		public void PrepareForInteraction()
		{
			// Debug.Log("Preparing for interaction...");
			if (interactor == null) throw new System.Exception("Interactor not assigned.");
			interactor.PrepareInteraction();

			foreach (var interactable in interactables)
			{
				interactable.PrepareInteraction();
			}
		}
		
		public void EndInteraction()
		{
			if (interactor == null) throw new System.Exception("Interactor not assigned.");
			interactor.EndInteraction();

			// foreach (var interactable in interactables)
			// {
			// 	interactable.EndInteraction();
			// }
		}
	}
}
