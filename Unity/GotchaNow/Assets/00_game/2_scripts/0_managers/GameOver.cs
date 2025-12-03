using System.Collections.Generic;
using GotchaNow;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Header("References")]
    public static GameOver instance;
    //public string currentBattle;

    public GameObject screenOverlay;
    public GameObject quotaScreen;
    public GameObject defeatedScreen;
    public GameObject neutralEndingScreen;
    // [SerializeField] private GameObject bottomBar;
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    [Space]
    [SerializeField] private List<GameObject> disableObjects;

    [HideInInspector]
    public float quotaState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        quotaState = 0;

        gameObject.SetActive(false);

        screenOverlay.SetActive(false);
        defeatedScreen.SetActive(false);
        quotaScreen.SetActive(false);

        //deactivate buttons
        // bottomBar.SetActive(false);
    }

    public void GameOverQuota()
    {
        Debug.Log("GameOverQuota called");
        gameObject.SetActive(true);

        screenOverlay.SetActive(true);
        quotaScreen.SetActive(true);
        defeatedScreen.SetActive(false);
        neutralEndingScreen.SetActive(false);

        //activate buttons
        // bottomBar.SetActive(true);

        // Disable
        foreach(var o in disableObjects)
        {
            if(!o) continue;
            o.SetActive(false);
        }
    }

    public void GameOverDefeated()
    {
        Debug.Log("GameOverDefeated called");
        gameObject.SetActive(true);

        screenOverlay.SetActive(true);
        quotaScreen.SetActive(false);
        defeatedScreen.SetActive(true);
        neutralEndingScreen.SetActive(false);

        //activate buttons
        // bottomBar.SetActive(true);

        // Disable
        foreach(var o in disableObjects)
        {
            if(!o) continue;
            o.SetActive(false);
        }
    }

    public void GameOverNeutralEnding()
    {
        Debug.Log("GameOverNeutralEnding called");
        gameObject.SetActive(true);

        screenOverlay.SetActive(true);
        quotaScreen.SetActive(false);
        defeatedScreen.SetActive(false);
        neutralEndingScreen.SetActive(true);

        //activate buttons
        // bottomBar.SetActive(true);

        // Disable
        foreach(var o in disableObjects)
        {
            if(!o) continue;
            o.SetActive(false);
        }
    }

    public void RetryBattle()
    {
        // Enable
        foreach(var o in disableObjects)
        {
            if(!o) continue;
            o.SetActive(true);
        }

        gameObject.SetActive(false);
        
        screenOverlay.SetActive(false);
        defeatedScreen.SetActive(false);
        quotaScreen.SetActive(false);

        //deactivate buttons
        // bottomBar.SetActive(false);

        NotificationManager.instance.currentQuota = quotaState;
        ProgressionManager.instance.StartBattle(ProgressionManager.instance.battleID);
    }

    public void GoToThanksForPlaying()
    {
        gameObject.SetActive(false);

        screenOverlay.SetActive(false);
        defeatedScreen.SetActive(false);
        quotaScreen.SetActive(false);

        //deactivate buttons
        // bottomBar.SetActive(false);

        ThanksForPlaying.Instance.ShowThanksScreen();
    }

    public void GoToTitle()
    {
        // Enable
        // foreach(var o in disableObjects)
        // {
        //     if(!o) continue;
        //     o.SetActive(true);
        // }

        // gameObject.SetActive(false);

        // screenOverlay.SetActive(false);
        // benchScreen.SetActive(false);
        // quotaScreen.SetActive(false);

        //deactivate buttons
        // bottomBar.SetActive(false);

        // quotaState = 0;

        //switch scene

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
