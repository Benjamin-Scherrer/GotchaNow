using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public string gameState;
    public string battleID;
    public string intermissionID;

    void Start()
    {
        
    }

    void Update()
    {

    }

    void StartIntermission(string id)
    {
        intermissionID = id;
        gameState = "intermission";

        if (intermissionID == "intro")
        {
            //
        }

        if (intermissionID == "tutorialCompleted")
        {
            //
        }

        if (intermissionID == "intermission1trueEndingPath")
        {
            //
        }

        if (intermissionID == "intermission1")
        {
            //
        }

        if (intermissionID == "secretBoss")
        {
            //
        }

        if (intermissionID == "trueEnding")
        {
            //
        }

        if (intermissionID == "neutralEnding")
        {
            //
        }

        if (intermissionID == "badEnding")
        {
            //
        }

    }

    void StartBattle(string id)
    {
        battleID = id;
        gameState = "battle";

        //set player state

        if (battleID == "tutorial")
        {
            //spawn tutorial enemy
        }

        if (battleID == "battle1")
        {
            //spawn regular enemy
        }

        if (battleID == "battle2trueEndingPath")
        {
            //spawn buffed regular enemy + minions
        }

        if (battleID == "battle2")
        {
            //spawn regular enemy + minions
        }
        
        if (battleID == "battle3")
        {
            //spawn queen
        }
    }
    
    void EndBattle(float achievedQuota, float goalQuota) //trigger when main enemy is defeated
    {   

        if (battleID == "tutorial")
        {
            //path to battle 1
        }

        if (battleID == "battle1")
        {
            if (achievedQuota == 0) //true path
            {

            }
            else //neutral/bad path
            {
                
            }
        }

        if (battleID == "battle2trueEndingPath")
        {
            if (achievedQuota == 0) //true path, secret boss
            {

            }
            else if (achievedQuota >= goalQuota) //bad ending
            {

            }
            else //neutral ending
            {
                
            }
        }

        if (battleID == "battle2")
        {
            if (achievedQuota >= goalQuota) //bad ending
            {

            }
            else //neutral ending
            {
                
            }
        }

        if (battleID == "battle3")
        {
            if (achievedQuota == 0) //true ending
            {

            }
            else //neutral ending
            {

            }
        }
        
    }
}
