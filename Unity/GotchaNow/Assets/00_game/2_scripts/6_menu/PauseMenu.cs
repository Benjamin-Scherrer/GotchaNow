using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GotchaNow
{
	public class PauseMenu : MonoBehaviour
	{
		private enum MenuState
		{
			DISABLED,
			PAUSEMENU,
			CONTROLS,
		}

		// Singleton instance
		public static PauseMenu Instance { get; private set; }

		[Header("Input Settings")]
		[SerializeField] private InputActionReference pauseInputAction;

		[Header("Pause Menu")]
		[SerializeField] private Canvas pauseMenuScreen;

		[Header("Quit Game")]
		[SerializeField] private string mainMenuScene = "MainMenuScene";

		[Header("Controls")]
		[SerializeField] private Canvas controlsScreen;
		[SerializeField] private Slider volumeSlider;

		//Private Variables
		private MenuState menuState;

		//Public Properties
		public string MainMenuScene
		{
			get
			{
				return mainMenuScene;
			}
			private set
			{
				mainMenuScene = value;
			}
		}

		public bool IsPaused
		{
			get
			{
				return menuState != MenuState.DISABLED;
			}
		}

		public void TogglePauseScreen() //input based
		{
			Debug.Log("Toggle Controls Screen");
			switch (menuState)
			{
				case MenuState.DISABLED:
					Time.timeScale = 0f; //pause time	
					menuState = MenuState.PAUSEMENU;
					pauseMenuScreen.gameObject.SetActive(true);
					break;
				case MenuState.PAUSEMENU:
					menuState = MenuState.DISABLED;
					Time.timeScale = 1f; //resume time
					pauseMenuScreen.gameObject.SetActive(false);
					break;
				case MenuState.CONTROLS:
					//if in controls screen, go back to pause menu
					menuState = MenuState.DISABLED;
					controlsScreen.gameObject.SetActive(false);
					pauseMenuScreen.gameObject.SetActive(false);
					Time.timeScale = 1f; //resume time
					break;
				default:
					throw new Exception($"Invalid menu state {menuState} in ToggleControlsScreen");
			}
		}

		public void ToggleControlsScreen()//button based
		{
			Debug.Log("Toggle Controls Screen");
			switch (menuState)
			{
				case MenuState.PAUSEMENU:
					menuState = MenuState.CONTROLS;
					controlsScreen.gameObject.SetActive(true);
					break;
				case MenuState.CONTROLS:
					menuState = MenuState.PAUSEMENU;
					controlsScreen.gameObject.SetActive(false);
					break;
				default:
					throw new Exception($"Invalid menu state {menuState} in ToggleControlsScreen");
			}
		}

		public void QuitGameSession()//button based
		{
			Debug.Log($"Quit Game Session, Scene name: {MainMenuScene}");
			//Make sure that time is working properly
			Time.timeScale = 1f;
			SceneManager.LoadScene(MainMenuScene);
		}

		//Private Methods
		private void Awake()
		{
			//Make sure that time is set to 1
			if (Time.timeScale != 1f)
			{
				Debug.LogWarning($"Time.timeScale was {Time.timeScale} in MainMenuManager Awake, resetting to 1f");
				Time.timeScale = 1f;
			}
			if (Instance != null) throw new Exception("There are multiple instances of the PauseMenu in the scene!");
			Instance = this;

			menuState = MenuState.DISABLED;

			if (pauseMenuScreen == null) throw new Exception($"pauseMenuScreen reference not set in inspector");
			if (controlsScreen == null) throw new Exception($"controlsScreen reference not set in inspector");

			pauseMenuScreen.gameObject.SetActive(false);
			controlsScreen.gameObject.SetActive(false);
		}

		private void Update()
		{
			//update volume every frame
			AudioListener.volume = volumeSlider.value;

			if (pauseInputAction.action.WasPerformedThisFrame())
			{
				switch (menuState)
				{
					case MenuState.DISABLED:
						if (Time.timeScale != 1) break; //safety check
						//don't open pause menu if notification menu is open
						if(NotificationManager.instance == null) 
							throw new Exception("NotificationManager instance is null, cannot check if menu is open");
						if(NotificationManager.instance.menuOpen) break;
						//if in gameplay, open pause menu
						TogglePauseScreen();
						ToggleControlsScreen();
						break;
					case MenuState.PAUSEMENU:
						//if in pause menu, close pause menu
						TogglePauseScreen();
						break;
					case MenuState.CONTROLS:
						//if in controls screen, go back to pause menu
						ToggleControlsScreen();
						break;
					default:
						break;
				}
				
			}
		}
	}
}
