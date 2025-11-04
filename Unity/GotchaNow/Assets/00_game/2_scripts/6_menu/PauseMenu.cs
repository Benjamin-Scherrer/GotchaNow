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
		public static PauseMenu PauseMenuInstance { get; private set; }

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
		private List<Canvas> menus;

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

		public void TogglePauseScreen() //input based
		{
			Debug.Log("Toggle Controls Screen");
			switch (menuState)
			{
				case MenuState.DISABLED:
					Time.timeScale = 0f; //pause time	
					menuState = MenuState.PAUSEMENU;
					ToggleMenu(pauseMenuScreen);
					break;
				case MenuState.PAUSEMENU:
					menuState = MenuState.DISABLED;
					Time.timeScale = 1f; //resume time
					ToggleMenu(null);
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
					ToggleMenu(controlsScreen);
					break;
				case MenuState.CONTROLS:
					menuState = MenuState.PAUSEMENU;
					ToggleMenu(pauseMenuScreen);
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
			if (PauseMenuInstance != null) throw new Exception("There are multiple instances of the PauseMenu in the scene!");
			PauseMenuInstance = this;

			menus = new List<Canvas>()
			{
			pauseMenuScreen,
			controlsScreen,
			};

			menuState = MenuState.DISABLED;

			if (pauseMenuScreen == null) throw new Exception($"pauseMenuScreen reference not set in inspector");
			if (controlsScreen == null) throw new Exception($"controlsScreen reference not set in inspector");
			ToggleMenu(null);
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
						//if in gameplay, open pause menu
						TogglePauseScreen();
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

		private void ToggleMenu(Canvas menuToShow)
		{
			foreach (Canvas menu in menus)
			{
				if (menu == menuToShow)
				{
					menu.gameObject.SetActive(true);
				}
				else
				{
					menu.gameObject.SetActive(false);
				}
			}
		}
	}
}
