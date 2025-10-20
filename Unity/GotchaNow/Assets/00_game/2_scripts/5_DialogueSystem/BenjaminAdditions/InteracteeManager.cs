using System.Collections.Generic;
using DialogueSystem.Runtime.Interaction;
using UnityEngine;

namespace GotchaNow
{
	public class InteracteeManager : MonoBehaviour
	{
		public static InteracteeManager Instance { get; private set; }

		private readonly List<InteractableDialogue> interactables = new();
		private void Awake()
		{
			if (Instance != null) throw new System.Exception("Multiple InteracteeManagers in scene.");
			Instance = this;
		}

		public void RegisterInteractable(InteractableDialogue interactable)
		{
			// Register the interactable
			if(interactables.Contains(interactable)) throw new System.Exception("Interactable already registered.");
			interactables.Add(interactable);
		}

		public void UnregisterInteractable(InteractableDialogue interactable)
		{
			// Unregister the interactable
			if (!interactables.Contains(interactable)) throw new System.Exception("Interactable not registered.");
			interactables.Remove(interactable);
		}

		public InteractableDialogue GetInteractableDialogue(Vector3 position, float maxDistance)
		{
			// Debug.Log("Searching for interactables...");
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
		
		public void PrepareAllInteractables()
		{
			foreach (var interactable in interactables)
			{
				interactable.PrepareInteraction();
			}
		}
	}
}
