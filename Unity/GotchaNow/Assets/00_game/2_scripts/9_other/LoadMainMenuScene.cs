using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace GotchaNow
{
	public class LoadMainMenuScene : MonoBehaviour
	{
		[Header("Load Main Menu Scene")]
		[SerializeField] private string mainMenuSceneName = "MainMenuScene";

		[SerializeField] private List<GameObject> dontDestroyOnLoadObjects;
		[SerializeField] private ShootScreenBuffer shootScreenBuffer;

		[Header("References")]
		[SerializeField] private Image backgroundImage;
		public void LoadMainMenu()
		{
			SceneManager.LoadScene(mainMenuSceneName);
		}

		private void Awake()
		{
			foreach (GameObject obj in dontDestroyOnLoadObjects)
			{
				DontDestroyOnLoad(obj);
			}
		}

		private void Start()
		{
			StartCoroutine(LoadAsynchronously(1));
		}

		private IEnumerator LoadAsynchronously(int sceneIndex)
		{
			// yield return StartCoroutine(LightenBackground());
			AsyncOperation process = SceneManager.LoadSceneAsync(sceneIndex);

			while (!process.isDone && process.progress < 0.5f)
			{
				yield return null;
			}

			Debug.Log("Starting splash screen");
			SplashScreen.Begin();
			SplashScreen.Draw();

			StartCoroutine(LightenBackground());
		
			while (SplashScreen.isFinished == false)
			{
				// SplashScreen.Draw();
				yield return null;
			}

			Debug.Log("Splashscreen finished");

			while (!process.isDone)
			{
				yield return null;
			}

			Debug.Log("Loading finished");

			// yield return shootScreenBuffer.ShootScreen(); 
			// Doesn't work causes screen flicker. 
			// Solution was to just have the second camera always active. 

			foreach (GameObject obj in dontDestroyOnLoadObjects)
			{
				Destroy(obj);
			}

			Debug.Log("Destroyed dontdestroyonload objects");
		}

		private IEnumerator LightenBackground()
		{
			Color goalColor = new(1f, 1f, 1f, 1f);
			Color initialColor = backgroundImage.color;
			float duration = 2f;
			float elapsed = 0f;
			while(elapsed < duration)
			{
				elapsed += Time.unscaledDeltaTime;
				backgroundImage.color = Color.Lerp(initialColor, goalColor, elapsed / duration);
				yield return null;
			}
			backgroundImage.color = goalColor;
		}
	}
}
