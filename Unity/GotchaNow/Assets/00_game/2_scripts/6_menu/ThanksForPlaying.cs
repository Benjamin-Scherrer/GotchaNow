using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace GotchaNow
{
	public class ThanksForPlaying : MonoBehaviour
	{
		public static ThanksForPlaying Instance;

		[Header("Inputs")]
		[SerializeField] private InputActionReference move;
		[SerializeField] private InputActionReference click;

		[Header("References")]
		[SerializeField] private string mainMenuSceneName = "MainMenuScene";

		[SerializeField] List<GameObject> disableGameObjects;

		[Header("UI")]
		[SerializeField] private GameObject ui;
		[SerializeField] private UnityEngine.UI.Button uiSkipButton;
		private bool buttonSelected = false;

		// PUBLIC
		public void ShowThanksScreen()
		{
			gameObject.SetActive(true);

			foreach (GameObject go in disableGameObjects)
			{
				if(!go) continue;
				go.SetActive(false);
			}	
		}

		// PRIVATE
		private void Awake()
        {
            gameObject.SetActive(false);
			
			if(Instance != null) throw new System.Exception("Multiple ThanksForPlaying instances detected!");
			Instance = this;

			ui.SetActive(false);
			buttonSelected = false;
        }

		private void Update()
        {	
			// Debug.Log("Video time: " + videoPlayer.time);  
			if (ui.activeInHierarchy == false)
            {
				if(move.action.WasPerformedThisFrame() || click.action.WasPerformedThisFrame())
				{
					ui.SetActive(true);
					return;
				}
				return;
            }
			if(ui.activeInHierarchy == true)
            {
				if(buttonSelected == false)
				{
					if (move.action.WasPerformedThisFrame())
					{
						uiSkipButton.OnDeselect(null);
						uiSkipButton.OnPointerExit(null);
						buttonSelected = false;
						ui.SetActive(false);
						return;
					}
					if (click.action.WasPerformedThisFrame())
					{
						uiSkipButton.Select();
						uiSkipButton.OnSelect(null);
						uiSkipButton.OnPointerEnter(null);
						buttonSelected = true;
						return;
					}
					return;
				}
                if (buttonSelected)
                {
                    if (move.action.WasPerformedThisFrame())
					{
						uiSkipButton.OnDeselect(null);
						uiSkipButton.OnPointerExit(null);
						buttonSelected = false;
						return;
					}
					if (click.action.WasPerformedThisFrame())
					{
						ReturnToMainMenu();
						return;
					}
					return;
                }
			}
        }

		private void ReturnToMainMenu()
		{
			MusicPlayer.instance.StopMusic();
			SceneManager.LoadScene(mainMenuSceneName);
		}
	}
}
