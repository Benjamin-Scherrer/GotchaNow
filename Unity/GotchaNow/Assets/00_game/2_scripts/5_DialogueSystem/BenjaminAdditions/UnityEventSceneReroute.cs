using System;
using DialogueSystem.Runtime.Narration;
using UnityEngine;

namespace GotchaNow
{
	public class UnityEventSceneReroute : MonoBehaviour
	{
        public void StartBattle(string id) //enable battle controls, enable enemies
		{
			// Debug.Log("Starting Battle: " + id);
			if (ProgressionManager.instance == null) throw new System.Exception("ProgressionManager instance is null");
			ProgressionManager.instance.StartBattle(id);
		}


		public void StartIntermission(String id) //disable battle controls, disable enemies
		{
			// Debug.Log("Starting Intermission: " + id);
			if (ProgressionManager.instance == null) throw new System.Exception("ProgressionManager instance is null");
			ProgressionManager.instance.StartIntermission(id);
		}
		
		public void SkipThroughAll() //for UnityEvent use
		{
			if (NarrativeController.Instance == null) throw new System.Exception("NarrativeController instance is null");
			while (NarrativeController.Instance.IsNarrating)
			{
				// Debug.Log("Manual Skip Dialogue called from UnityEventSceneReroute");
				NarrativeController.Instance.NextNarrative();
			}
		}

		public void GameOverBench()
		{
			if (NarrativeController.Instance == null) throw new System.Exception("NarrativeController instance is null");
			GameOver.instance.GameOverBench();
		}

		public void GameOverQuota()
		{
			if (NarrativeController.Instance == null) throw new System.Exception("NarrativeController instance is null");
			GameOver.instance.GameOverQuota();
		}

		public void EnablePhoneView()
		{
			PhoneViewController.instance.EnablePhoneView();
		}
		
		public void DialogleBoxAlign(String alignment)
		{
			switch (alignment)
			{
				case "bottom":
					throw new NotImplementedException("Bottom alignment not implemented yet");
					// break;
				case "top":
					throw new NotImplementedException("Top alignment not implemented yet");
					// break;
				default:
					Debug.LogError("Invalid alignment parameter passed to DialogleBoxAlign: " + alignment);
					break;
			}
		}	
	}
}
