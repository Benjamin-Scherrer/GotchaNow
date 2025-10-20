using GotchaNow;
using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
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
    public TextMeshProUGUI debugText;
    public GameObject player;
    public GameObject boss;
    public GameObject minion;
    public GameObject queen;
    public Transform bossSpawnPoint;
    public Transform queenSpawnPoint;
    private bool skipReady = true;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        bm = BattleManager.instance;
        nm = NotificationManager.instance;
        
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

        player.GetComponent<PlayerBattle>().enabled = false;
        player.GetComponent<PlayerIntermission>().enabled = true;

        if (intermissionID == "intro")
        {   
            nextState = "intermission";
            nextID = "preBattle1";

            debugText.text += "\n\nwelcome, new employee. in this game, heroes fight monsters\nfor the entertainment of gamers.\nget your user to spend money on you";

            nm.FullReset();
            
            queen.SetActive(true);
            boss.SetActive(false);

            Vector3 spawnPoint = queenSpawnPoint.position;

            queen.transform.position = spawnPoint;
            queen.transform.eulerAngles = new Vector3(0, 180, 0);


            //Dialogue Update
            InteracteeManager.Instance.PrepareAllInteractables();
            //Force start dialogue
            intermissionDialogue.Interact();

            return;
        }

        if (intermissionID == "preBattle1") //after tutorial, before 1st battle
        {
            nextState = "battle";
            nextID = "battle1";

            debugText.text += "\n\nget ready for your first fight";

            boss.SetActive(true);

            Vector3 spawnPoint = bossSpawnPoint.position;

            boss.transform.position = spawnPoint;
            boss.transform.eulerAngles = new Vector3(0, 180, 0);

            queen.transform.position = spawnPoint + new Vector3(8, 0, 1);

            //Dialogue Update
            InteracteeManager.Instance.PrepareAllInteractables();
            //Force start dialogue
            intermissionDialogue.Interact();
            
            return;
        }

        if (intermissionID == "intermission1TrueEndingPath") //won 1st battle with 0 quota
        {
            nextState = "battle";
            nextID = "battle2trueEndingPath";

            debugText.text += "\n\nyou should make the user spend some money";

            queen.SetActive(true);

            //Dialogue Update
            InteracteeManager.Instance.PrepareAllInteractables();
            //Force start dialogue
            intermissionDialogue.Interact();

            return;
        }

        if (intermissionID == "intermission1") //won 1st battle with some quota
        {
            nextState = "battle";
            nextID = "battle2";

            debugText.text += "\n\nyou're a good employee.\nkeep going, squeeze that user'";

            queen.SetActive(true);

            //Dialogue Update
            InteracteeManager.Instance.PrepareAllInteractables();
            //Force start dialogue
            intermissionDialogue.Interact();

            return;
        }

        if (intermissionID == "preSecretBoss") //won 2nd battle with 0 quota, queen steps in
        {
            nextState = "battle";
            nextID = "battle3";

            debugText.text += "\n\nwhat do you think you're doing\ni will have to step in";

            Vector3 spawnPoint = bossSpawnPoint.position;

            boss.transform.position = spawnPoint + new Vector3(0, 0, 2);

            queen.SetActive(true);

            queen.transform.position = spawnPoint;
            queen.transform.eulerAngles = new Vector3(0, 180, 0);

            //Dialogue Update
            InteracteeManager.Instance.PrepareAllInteractables();
            //Force start dialogue
            intermissionDialogue.Interact();

            return;
        }

        if (intermissionID == "trueEnding") //defeated secret boss with 0 quota
        {
            nextState = "intermission";
            nextID = "intro";

            debugText.text += "\n\ngame world deleted. user can't connect to server";

            queen.SetActive(false);

            //Dialogue Update
            InteracteeManager.Instance.PrepareAllInteractables();
            //Force start dialogue
            intermissionDialogue.Interact();

            return;
        }

        if (intermissionID == "trueEndingFailed") //defeated secret boss with some quota
        {
            nextState = "intermission";
            nextID = "neutralEnding";

            debugText.text += "\n\nyou may have defeated me but not capitalism";

            //Dialogue Update
            InteracteeManager.Instance.PrepareAllInteractables();
            //Force start dialogue
            intermissionDialogue.Interact();

            return;
        }

        if (intermissionID == "neutralEnding") //won 2nd battle with some quota
        {
            nextState = "intermission";
            nextID = "intro";

            debugText.text += "\n\ndecent job. we got a new paying user thanks to you\nmight have to replace you with someone better tho";


            //Dialogue Update
            InteracteeManager.Instance.PrepareAllInteractables();
            //Force start dialogue
            intermissionDialogue.Interact();

            return; 
        }

        if (intermissionID == "badEnding") //won 2nd battle with full quota
        {
            nextState = "intermission";
            nextID = "intro";

            debugText.text += "\n\nyou're our new top employee. incredible work";

            //Dialogue Update
            InteracteeManager.Instance.PrepareAllInteractables();
            //Force start dialogue
            intermissionDialogue.Interact();

            return;
        }

        if (intermissionID == "gameOver") //lost battle (any)
        {
            nextState = "intermission";
            nextID = "intro";

            debugText.text += "\n\nyou died";

            //Dialogue Update
            InteracteeManager.Instance.PrepareAllInteractables();
            //Force start dialogue
            intermissionDialogue.Interact();

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
        player.GetComponent<PlayerIntermission>().enabled = false;

        if (battleID == "tutorial")
        {
            //spawn tutorial enemy
            return;
        }

        if (battleID == "battle1")
        {
            queen.SetActive(false);

            //Beni debug test
            boss.SetActive(true);
            //test end
            
            boss.GetComponent<BossEnemy>().enabled = true;
            boss.GetComponent<EnemyIntermission>().enabled = false;

            boss.GetComponent<Enemy>().StartBattle();

            Vector3 spawnPoint = bossSpawnPoint.position;
            boss.transform.position = spawnPoint;
            boss.transform.eulerAngles = new Vector3(0, 180, 0);

            boss.GetComponent<Enemy>().isMainEnemy = true;

            return;
        }

        if (battleID == "battle2trueEndingPath")
        {
            queen.SetActive(false);
            
            boss.GetComponent<BossEnemy>().enabled = true;
            boss.GetComponent<EnemyIntermission>().enabled = false;

            boss.GetComponent<Enemy>().StartBattle();

            Vector3 spawnPoint = bossSpawnPoint.position;
            boss.transform.position = spawnPoint;
            boss.transform.eulerAngles = new Vector3(0, 180, 0);

            boss.GetComponent<Enemy>().isMainEnemy = true;

            GameObject minion1 = Instantiate(minion);
            GameObject minion2 = Instantiate(minion);

            minion1.transform.position = spawnPoint + new Vector3(-4, 0, 0);
            minion1.transform.eulerAngles = new Vector3(0, 180, 0);

            minion2.transform.position = spawnPoint + new Vector3(4, 0, 0);
            minion2.transform.eulerAngles = new Vector3(0, 180, 0);

            return;
        }

        if (battleID == "battle2")
        {
            queen.SetActive(false);
            
            boss.GetComponent<BossEnemy>().enabled = true;
            boss.GetComponent<EnemyIntermission>().enabled = false;

            boss.GetComponent<Enemy>().StartBattle();

            Vector3 spawnPoint = bossSpawnPoint.position;
            boss.transform.position = spawnPoint;
            boss.transform.eulerAngles = new Vector3(0, 180, 0);

            boss.GetComponent<Enemy>().isMainEnemy = true;

            GameObject minion1 = Instantiate(minion);
            GameObject minion2 = Instantiate(minion);

            minion1.transform.position = spawnPoint + new Vector3(-6, 0, 0);
            minion1.transform.eulerAngles = new Vector3(0, 180, 0);

            minion2.transform.position = spawnPoint + new Vector3(6, 0, 0);
            minion2.transform.eulerAngles = new Vector3(0, 180, 0);

            return;
        }

        if (battleID == "battle3")
        {
            boss.SetActive(false);

            queen.GetComponent<QueenEnemy>().enabled = true;
            //queen.GetComponent<EnemyIntermission>().enabled = false;

            queen.GetComponent<Enemy>().StartBattle();

            Vector3 spawnPoint = queenSpawnPoint.position;
            queen.transform.position = spawnPoint;
            queen.transform.eulerAngles = new Vector3(0, 180, 0);

            queen.GetComponent<Enemy>().isMainEnemy = true;

            return;
        }
        
        throw new System.Exception("Invalid battle ID: " + battleID);
    }

    public void EndBattle(float achievedQuota, float goalQuota) //trigger when main enemy is defeated
    {

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
        sendNotifUI.SetActive(true);
        battleUI.SetActive(true);

        nm.StartBattle();
        StartCoroutine(bm.UpdatePlayerHP(0, 1));
    }

    private void EnableIntermissionUI()
    {
        sendNotifUI.SetActive(false);
        battleUI.SetActive(false);
    }
}
