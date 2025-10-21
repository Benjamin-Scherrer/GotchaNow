using DialogueSystem.Runtime.Narration;
using UnityEngine;

namespace GotchaNow
{
	public class UnityEventSceneReroute : MonoBehaviour
	{
        public void StartBattle(string id) //enable battle controls, enable enemies
		{
			Debug.Log("Starting Battle: " + id);
			if (ProgressionManager.instance == null) throw new System.Exception("ProgressionManager instance is null");
			ProgressionManager.instance.StartBattle(id);
		}

		public void SkipThroughAll() //for UnityEvent use
		{
			if (NarrativeController.instance == null) throw new System.Exception("NarrativeController instance is null");
			while (NarrativeController.instance.IsNarrating)
			{
				Debug.Log("Manual Skip Dialogue called from UnityEventSceneReroute");
				NarrativeController.instance.NextNarrative();
			}
		}

		public void enablePhoneView()
        {
			PhoneViewController.instance.EnablePhoneView();
        }
	}
}
