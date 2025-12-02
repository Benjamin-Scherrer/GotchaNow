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
    public GameObject benchScreen;
    [SerializeField] private GameObject bottomBar;
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    [HideInInspector]
    public float quotaState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        quotaState = 0;

        gameObject.SetActive(false);

        screenOverlay.SetActive(false);
        benchScreen.SetActive(false);
        quotaScreen.SetActive(false);

        //deactivate buttons
        bottomBar.SetActive(false);
    }

    public void GameOverQuota()
    {
        gameObject.SetActive(true);

        screenOverlay.SetActive(true);
        quotaScreen.SetActive(true);
        benchScreen.SetActive(false);

        //activate buttons
        bottomBar.SetActive(true);
    }

    public void GameOverBench()
    {
        gameObject.SetActive(true);

        screenOverlay.SetActive(true);
        quotaScreen.SetActive(false);
        benchScreen.SetActive(true);

        //activate buttons
        bottomBar.SetActive(true);
    }

    public void RetryBattle()
    {
        gameObject.SetActive(false);
        
        screenOverlay.SetActive(false);
        benchScreen.SetActive(false);
        quotaScreen.SetActive(false);

        //deactivate buttons
        bottomBar.SetActive(false);

        NotificationManager.instance.currentQuota = quotaState;
        ProgressionManager.instance.StartBattle(ProgressionManager.instance.battleID);
    }

    public void GoToTitle()
    {
        // gameObject.SetActive(false);

        // screenOverlay.SetActive(false);
        // benchScreen.SetActive(false);
        // quotaScreen.SetActive(false);

        //deactivate buttons
        bottomBar.SetActive(false);

        // quotaState = 0;

        //switch scene

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
