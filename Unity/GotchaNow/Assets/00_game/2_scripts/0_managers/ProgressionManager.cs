using GotchaNow;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager instance;
    public InputActionReference skipInput;
    private BattleManager bm;
    private NotificationManager nm;
    public string gameState;
    public string battleID;
    public string intermissionID;
    private string nextState;
    private string nextID;
    public GameObject intermissionUI;
    public IntermissionDialogue intermissionDialogue;

    public GameObject battleUI;
    public GameObject sendNotifUI;
    public NotificationManager notificationManager;
    public TextMeshProUGUI debugText;
    public GameObject player;
    public GameObject boss;
    public GameObject minion;
    public GameObject queen;
    public GameObject minionSpawner;
    public Vector3 playerSpawnPoint;
    public Vector3 bossSpawnPoint;
    public Vector3 queenSpawnPoint;
    public Transform arenaCenter;
    private bool skipReady = true;
    public UnityEvent EndBattleEvent;

    void Awake()
    {
        instance = this;

        EndBattleEvent = new UnityEvent();
    }

    void Start()
    {
        bm = BattleManager.instance;
        nm = NotificationManager.instance;

        InteracteeManager.Instance.EndInteraction();
        
        if (gameState == "intermission") //start with inspector settings
        {
            StartIntermission(intermissionID);
        }
        
        else if (gameState == "battle") //start with inspector settings
        {
            StartBattle(battleID);
        }
    }

    void Update()
    {
        if (gameState == "intermission")
        {
            if (skipInput.action.IsPressed() && skipReady) //debug : skip to next state
            {
                Skip(nextState, nextID);
            }

            skipReady = false;
        }

        if (!skipReady) //re-enable input when button released
        {
            if (!skipInput.action.IsPressed())
            {
                skipReady = true;
            }
        }
    }
    
    private void Skip(string state, string id) //debug function. skip to next state
    {
        if (state == "intermission")
        {
            StartIntermission(id);
        }
        else if (state == "battle")
        {
            StartBattle(id);
        }
    }

    public void StartIntermission(string id) //disable battle controls, story scenes
    {
        intermissionID = id;
        gameState = "intermission";

        debugText.text = "gameState : " + gameState + "\nID : " + intermissionID + "\npress N or Start : skip";

        EnableIntermissionUI();
        MusicPlayer.instance.PlayIntermissionMusic();
        

        if (intermissionID == "intro")
        {   
            nextState = "intermission";
            nextID = "preBattle1";

            debugText.text += "\n\nwelcome, new employee. in this game, heroes fight monsters\nfor the entertainment of gamers.\nget your user to spend money on you";

            nm.FullReset();
            
            queen.SetActive(true);
            boss.SetActive(true);

            queenSpawnPoint = arenaCenter.position + new Vector3(-3,2,8);   
            bossSpawnPoint = arenaCenter.position + new Vector3(3,2,8);       

            queen.transform.position = queenSpawnPoint;
            queen.transform.eulerAngles = new Vector3(0, 180, 0);

            boss.transform.position = bossSpawnPoint;
            boss.transform.eulerAngles = new Vector3(0, 180, 0);

            //Dialogue Update
            InteracteeManager.Instance.PrepareForInteraction();
            //Force start dialogue
            intermissionDialogue.Interact();

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        if (intermissionID == "preBattle1") //after tutorial, before 1st battle
        {
            nextState = "battle";
            nextID = "battle1";

            debugText.text += "\n\nget ready for your first fight";

            boss.SetActive(true);

            bossSpawnPoint = arenaCenter.position + new Vector3(3,2,8);
            queenSpawnPoint = arenaCenter.position + new Vector3(-3,2,8);

            boss.transform.position = bossSpawnPoint;
            boss.transform.eulerAngles = new Vector3(0, 180, 0);

            //queen.transform.position = queenSpawnPoint;
            //queen.transform.eulerAngles = new Vector3(0, 180, 0);

             //Dialogue Update
            InteracteeManager.Instance.PrepareForInteraction();
            //Force start dialogue
            intermissionDialogue.Interact();

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();
            
            return;
        }

        if (intermissionID == "intermission1TrueEndingPath") //won 1st battle with 0 quota
        {
            nextState = "battle";
            nextID = "battle2trueEndingPath";

            debugText.text += "\n\nyou should make the user spend some money";

            queen.SetActive(true);

            //Dialogue Update
            InteracteeManager.Instance.PrepareForInteraction();
            //Force start dialogue
            intermissionDialogue.Interact();

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        if (intermissionID == "intermission1") //won 1st battle with some quota
        {
            nextState = "battle";
            nextID = "battle2";

            debugText.text += "\n\nyou're a good employee.\nkeep going, squeeze that user'";

            queen.SetActive(true);

            //Dialogue Update
            InteracteeManager.Instance.PrepareForInteraction();
            //Force start dialogue
            intermissionDialogue.Interact();

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        if (intermissionID == "preSecretBoss") //won 2nd battle with 0 quota, queen steps in
        {
            nextState = "battle";
            nextID = "battle3";

            debugText.text += "\n\nwhat do you think you're doing\ni will have to step in";

            //Vector3 spawnPoint = bossSpawnPoint.position;

            //boss.transform.position = spawnPoint + new Vector3(0, 0, 2);

            queen.SetActive(true);

            //Dialogue Update
            InteracteeManager.Instance.PrepareForInteraction();
            //Force start dialogue
            intermissionDialogue.Interact();

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        if (intermissionID == "trueEnding") //defeated secret boss with 0 quota
        {
            nextState = "intermission";
            nextID = "intro";

            debugText.text += "\n\ngame world deleted. user can't connect to server";

            queen.SetActive(false);

            //Dialogue Update
            InteracteeManager.Instance.PrepareForInteraction();
            //Force start dialogue
            intermissionDialogue.Interact();

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        if (intermissionID == "trueEndingFailed") //defeated secret boss with some quota
        {
            nextState = "intermission";
            nextID = "neutralEnding";

            debugText.text += "\n\nyou may have defeated me but not capitalism";

            //Dialogue Update
            InteracteeManager.Instance.PrepareForInteraction();
            //Force start dialogue
            intermissionDialogue.Interact();

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        if (intermissionID == "neutralEnding") //won 2nd battle with some quota
        {
            nextState = "intermission";
            nextID = "intro";

            debugText.text += "\n\ndecent job. we got a new paying user thanks to you\nmight have to replace you with someone better tho";

             //Dialogue Update
            InteracteeManager.Instance.PrepareForInteraction();
            //Force start dialogue
            intermissionDialogue.Interact();

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return; 
        }

        if (intermissionID == "badEnding") //won 2nd battle with full quota
        {
            EndBattleEvent.Invoke();
            
            nextState = "intermission";
            nextID = "intro";

            debugText.text += "\n\nyou're our new top employee. incredible work";

            GameOver.instance.GameOverQuota();

            for (int i = 0; i < bm.activeEnemy.Count; i++) //deactivate minions
            {   
                if (bm.activeEnemy[i].GetComponent<Enemy>().enemyType == "minion")
                {
                    bm.activeEnemy[i].GetComponent<MinionEnemy>().EndBattle();
                    i -= 1;
                }
            }

            boss.SetActive(false); //debug
            queen.SetActive(false); //debug

            //Dialogue Update
            InteracteeManager.Instance.PrepareForInteraction();
            //Force start dialogue
            //intermissionDialogue.Interact();

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        if (intermissionID == "gameOver") //lost battle (any)
        {
            EndBattleEvent.Invoke();
            
            nextState = "intermission";
            nextID = "intro";

            debugText.text += "\n\nyou died";

            GameOver.instance.GameOverBench();

            for (int i = 0; i < bm.activeEnemy.Count; i++) //deactivate minions
            {   
                if (bm.activeEnemy[i].GetComponent<Enemy>().enemyType == "minion")
                {
                    bm.activeEnemy[i].GetComponent<MinionEnemy>().EndBattle();
                    i -= 1;
                }
            }

            boss.SetActive(false); //debug
            queen.SetActive(false); //debug

            //Dialogue Update
            InteracteeManager.Instance.PrepareForInteraction();
            //Force start dialogue
            //intermissionDialogue.Interact();

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        throw new System.Exception("Invalid intermission ID: " + intermissionID);
    }

    public void StartBattle(string id) //enable battle controls, enable enemies
    {
        battleID = id;
        gameState = "battle";

        debugText.text = "gameState : " + gameState + "\nID : " + battleID;

        EnableBattleUI();

        player.GetComponent<PlayerBattle>().enabled = true;
        player.GetComponent<PlayerBattle>().StartBattle();
        player.GetComponent<PlayerIntermission>().enabled = false;

        //End intermission dialogue if still active
        InteracteeManager.Instance.EndInteraction();

        if (battleID == "tutorial")
        {
            //spawn tutorial enemy

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        if (battleID == "battle1")
        {
            MusicPlayer.instance.PlayKatoroMusic();
            
            queen.SetActive(false);
            boss.SetActive(true);

            boss.GetComponent<EnemyIntermission>().EndIntermission();
            boss.GetComponent<Enemy>().StartBattle();

            bossSpawnPoint = arenaCenter.position + new Vector3(0,2,8);
            playerSpawnPoint = arenaCenter.position + new Vector3(0,2,-8);

            boss.transform.position = bossSpawnPoint;
            boss.transform.eulerAngles = new Vector3(0, 180, 0);

            player.transform.position = playerSpawnPoint;

            boss.GetComponent<Enemy>().isMainEnemy = true;

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        if (battleID == "battle2trueEndingPath")
        {
            MusicPlayer.instance.PlayKatoroMusic();
            
            queen.SetActive(false);
            boss.SetActive(true);
            
            boss.GetComponent<EnemyIntermission>().EndIntermission();
            boss.GetComponent<Enemy>().StartBattle();

            bossSpawnPoint = arenaCenter.position + new Vector3(0,2,8);
            playerSpawnPoint = arenaCenter.position + new Vector3(0,2,-8);

            boss.transform.position = bossSpawnPoint;
            boss.transform.eulerAngles = new Vector3(0, 180, 0);

            player.transform.position = playerSpawnPoint;

            boss.GetComponent<Enemy>().isMainEnemy = true;

            GameObject spawnRing1 = Instantiate(minionSpawner, arenaCenter.position + Vector3.up * 4 + Vector3.right * 4, transform.rotation);
            spawnRing1.GetComponent<MagicRingAttack>().spawner = true;

            GameObject spawnRing2 = Instantiate(minionSpawner, arenaCenter.position + Vector3.up * 4 - Vector3.right * 4, transform.rotation);
            spawnRing2.GetComponent<MagicRingAttack>().spawner = true;

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        if (battleID == "battle2")
        {
            MusicPlayer.instance.PlayKatoroMusic();
            
            queen.SetActive(false);
            boss.SetActive(true);
            
            boss.GetComponent<EnemyIntermission>().EndIntermission();
            boss.GetComponent<Enemy>().StartBattle();

            bossSpawnPoint = arenaCenter.position + new Vector3(0,2,8);
            playerSpawnPoint = arenaCenter.position + new Vector3(0,2,-8);

            boss.transform.position = bossSpawnPoint;
            boss.transform.eulerAngles = new Vector3(0, 180, 0);

            player.transform.position = playerSpawnPoint;

            boss.GetComponent<Enemy>().isMainEnemy = true;

            GameObject spawnRing1 = Instantiate(minionSpawner, arenaCenter.position + Vector3.up * 4 + Vector3.right * 4, transform.rotation);
            spawnRing1.GetComponent<MagicRingAttack>().spawner = true;

            GameObject spawnRing2 = Instantiate(minionSpawner, arenaCenter.position + Vector3.up * 4 - Vector3.right * 4, transform.rotation);
            spawnRing2.GetComponent<MagicRingAttack>().spawner = true;

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }

        if (battleID == "battle3")
        {
            MusicPlayer.instance.PlayAyaMusic();
            
            boss.SetActive(false);
            queen.SetActive(true);

            queen.GetComponent<QueenEnemy>().enabled = true;
            //queen.GetComponent<EnemyIntermission>().enabled = false;
            queen.GetComponent<Enemy>().StartBattle();

            queenSpawnPoint = arenaCenter.position + new Vector3(0,2,8);
            playerSpawnPoint = arenaCenter.position + new Vector3(0,2,-8);

            queen.transform.position = queenSpawnPoint;
            queen.transform.eulerAngles = new Vector3(0, 180, 0);

            player.transform.position = playerSpawnPoint;

            queen.GetComponent<Enemy>().isMainEnemy = true;

            //Start Chat Message History
            ChatMessagesManager.Instance.DisplayMessageHistory();

            return;
        }
        
        throw new System.Exception("Invalid battle ID: " + battleID);
    }

    public void EndBattle(float achievedQuota, float goalQuota) //trigger when main enemy is defeated
    {
        EndBattleEvent.Invoke();
        player.GetComponent<PlayerBattle>().EndBattle(); //clean up maybe

        if (battleID == "tutorial")
        {
            //path to battle 1

            StartIntermission("tutorialCompleted");

            return;
        }

        if (battleID == "battle1")
        {
            if (achievedQuota == 0) //true path
            {
                StartIntermission("intermission1TrueEndingPath");
            }
            else //neutral/bad path
            {
                StartIntermission("intermission1");
            }

            return;
        }

        if (battleID == "battle2trueEndingPath")
        {
            //Debug.Log("EndBattle()");
            
            for (int i = 0; i < bm.activeEnemy.Count; i++) //deactivate minions
            {
                //Debug.Log(bm.activeEnemy[i].GetComponent<Enemy>().enemyType);
                
                if (bm.activeEnemy[i].GetComponent<Enemy>().enemyType == "minion")
                {
                    bm.activeEnemy[i].GetComponent<MinionEnemy>().EndBattle();
                    i -= 1;
                }
            }

            if (achievedQuota == 0) //true path, secret boss
            {
                StartIntermission("preSecretBoss");
            }
            else if (achievedQuota >= goalQuota) //bad ending
            {
                StartIntermission("badEnding");
            }
            else //neutral ending
            {
                StartIntermission("neutralEnding");
            }
            
            return;
        }

        if (battleID == "battle2")
        {
            for (int i = 0; i < bm.activeEnemy.Count; i++) //deactivate minions
            {
                //Debug.Log(bm.activeEnemy[i].GetComponent<Enemy>().enemyType);
                
                if (bm.activeEnemy[i].GetComponent<Enemy>().enemyType == "minion")
                {
                    bm.activeEnemy[i].GetComponent<MinionEnemy>().EndBattle();
                    i -= 1;
                }
            }

            if (achievedQuota >= goalQuota) //bad ending
            {
                StartIntermission("badEnding");
            }
            else //neutral ending
            {
                StartIntermission("neutralEnding");
            }
            
            return;
        }

        if (battleID == "battle3")
        {
            if (achievedQuota == 0) //true ending
            {
                StartIntermission("trueEnding");
            }
            else //neutral ending
            {
                StartIntermission("trueEndingFailed");
            }

            return;
        }
        throw new System.Exception("Invalid intermission ID after battle, which should not be possible as the battle only starts with a valid ID");
    }

    private void EnableBattleUI()
    {
        notificationManager.enabled = true;
        
        sendNotifUI.SetActive(true);
        battleUI.SetActive(true);
        
        nm.StartBattle();
        StartCoroutine(bm.UpdatePlayerHP(0, 1));
    }

    private void EnableIntermissionUI()
    {
        notificationManager.enabled = false;
        
        sendNotifUI.SetActive(false);
        battleUI.SetActive(false);
    }
}
