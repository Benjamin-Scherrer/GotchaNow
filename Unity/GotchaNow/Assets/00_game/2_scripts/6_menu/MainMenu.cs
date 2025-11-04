using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    private enum MenuState
    {
        MAINMENU,
        CONTROLS,
        CREDITS,
    }

    // Singleton instance
    public static MainMenu MainMenuInstance { get; private set; }

    [Header("Input Actions")]
    [SerializeField] private InputActionReference pauseInputAction;

    [Header("Main Menu")]
    [SerializeField] private Canvas mainMenuScreen;

    [Header("Start Game")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Controls")]
    [SerializeField] private Canvas controlsScreen;
    [SerializeField] private Slider volumeSlider;

    [Header("Credits")]
    [SerializeField] private Canvas creditsScreen;

    //Private Variables
    private MenuState menuState;
    private List<Canvas> menus;

    public string GameSceneName
    {
        get
        {
            return gameSceneName;
        }
        private set
        {
            gameSceneName = value;
        }
    }

    public void StartGameButton()//button based
    {
        Debug.Log($"Start Game, Scene name: {GameSceneName}");
        //Make sure that time is working properly
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameSceneName);
    }

    public void ToggleControlsScreen()//button based
    {
        Debug.Log("Toggle Controls Screen");
        switch (menuState)
        {
            case MenuState.MAINMENU:
                menuState = MenuState.CONTROLS;
                ToggleMenu(controlsScreen);
                break;
            case MenuState.CONTROLS:
                menuState = MenuState.MAINMENU;
                ToggleMenu(mainMenuScreen);
                break;
            default:
                throw new Exception($"Invalid menu state {menuState} in ToggleControlsScreen");
        }
    }

    public void ToggleCreditsScreen()//button based
    {
        Debug.Log("Toggle Credits Screen");
        switch (menuState)
        {
            case MenuState.MAINMENU:
                menuState = MenuState.CREDITS;
                ToggleMenu(creditsScreen);
                break;
            case MenuState.CREDITS:
                menuState = MenuState.MAINMENU;
                ToggleMenu(mainMenuScreen);
                break;
            default:
                throw new Exception($"Invalid menu state {menuState} in ToggleCreditsScreen");
        }
    }
    
    public void QuitGame()//button based
    {
        Debug.Log("Quit Game");
        Application.Quit();
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
        if (MainMenuInstance != null) throw new Exception("There are multiple instances of the MainMenuManager in the scene!");
        MainMenuInstance = this;

        menus = new List<Canvas>()
        {
            mainMenuScreen,
            controlsScreen,
            creditsScreen,
        };

        menuState = MenuState.MAINMENU;

        if (mainMenuScreen == null) throw new Exception($"mainMenu reference not set in inspector");
        if (creditsScreen == null) throw new Exception($"creditsScreen reference not set in inspector");
        if (controlsScreen == null) throw new Exception($"controlsScreen reference not set in inspector");
        //show main menu & hide options and credits screen
        ToggleMenu(mainMenuScreen);
    }

    private void Update()
    {
        //update volume every frame
        AudioListener.volume = volumeSlider.value;

        if (pauseInputAction.action.WasPerformedThisFrame())
        {
            switch (menuState)
            {
                case MenuState.MAINMENU:
                    //if in gameplay, open pause menu
                    ToggleControlsScreen();
                    break;
                case MenuState.CONTROLS:
                    //if in pause menu, close pause menu
                    ToggleControlsScreen();
                    break;
                case MenuState.CREDITS:
                    //if in controls screen, go back to pause menu
                    ToggleCreditsScreen();
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
