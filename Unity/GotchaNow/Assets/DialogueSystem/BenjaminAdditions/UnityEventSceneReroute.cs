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
	}
}
