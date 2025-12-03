using UnityEngine;
using UnityEngine.SceneManagement;

namespace GotchaNow
{
	public class ThanksForPlaying : MonoBehaviour
	{
		public static ThanksForPlaying Instance;

		[Header("References")]
		[SerializeField] private string mainMenuSceneName = "MainMenuScene";

		// PUBLIC
		public void ShowThanksScreen()
		{
			gameObject.SetActive(true);
		}
		
		public void ReturnToMainMenu()
		{
			SceneManager.LoadScene(mainMenuSceneName);
		}

		// PRIVATE
		private void Awake()
        {
            gameObject.SetActive(false);
			
			if(Instance != null) throw new System.Exception("Multiple ThanksForPlaying instances detected!");
			Instance = this;
        }
	}
}
