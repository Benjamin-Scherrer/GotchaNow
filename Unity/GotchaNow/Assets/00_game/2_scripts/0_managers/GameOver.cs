using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver instance;
    //public string currentBattle;
    public GameObject screenOverlay;
    public GameObject quotaScreen;
    public GameObject benchScreen;
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
    }

    public void GameOverQuota()
    {
        gameObject.SetActive(true);

        screenOverlay.SetActive(true);
        quotaScreen.SetActive(true);

        //activate buttons
    }

    public void GameOverBench()
    {
        gameObject.SetActive(true);
        
        screenOverlay.SetActive(true);
        benchScreen.SetActive(true);

        //activate buttons
    }

    public void RetryBattle()
    {
        screenOverlay.SetActive(false);
        benchScreen.SetActive(false);
        quotaScreen.SetActive(false);

        //deactivate buttons

        NotificationManager.instance.currentQuota = quotaState;
        ProgressionManager.instance.StartBattle(ProgressionManager.instance.battleID);
    }

    public void GoToTitle()
    {
        screenOverlay.SetActive(false);
        benchScreen.SetActive(false);
        quotaScreen.SetActive(false);

        quotaState = 0;

        //switch scene
    }
}
