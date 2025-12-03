using UnityEngine;
using System.Collections.Generic;
using UnityEditor.UIElements;

public class Enemy : MonoBehaviour
{
    //essential values
    public string enemyType;
    public float maxHP = 100;
    public float HP = 100;
    public float knockback = 0;
    public int proximity = 0;
    public ProgressionManager pm;
    public NotificationManager nm;
    public bool isLockOnTarget = false;
    public bool isMainEnemy = false;

    [HideInInspector] public bool hit = false;

    //public GameObject dmgNumbers;
    //private GameObject HitBloom;

    private void Start()
    {
        pm = ProgressionManager.instance;
        nm = NotificationManager.instance;

        //BattleManager.instance.AddToEnemyList(this.gameObject);
    }

    private void OnEnable()
    {
        // pm = ProgressionManager.instance;
        // nm = NotificationManager.instance;

        if (BattleManager.instance != null)
        {
            BattleManager.instance.AddToEnemyList(this.gameObject);
        }
    }

    private void OnDisable()
    {    
        BattleManager.instance.RemoveFromEnemyList(this.gameObject);
    }

    private void FixedUpdate()
    {
        /* if (HP <= 0) //check if alive
        {
            //Destroy(gameObject);
        } */
    }

    public void HitByAttack(float dmg, float atkKnockback)
    {
        hit = true;
        HP -= dmg;
        knockback = atkKnockback;

        if (enemyType == "boss")
        {
            StartCoroutine(GetComponent<BossEnemy>().GotHit(atkKnockback));
        }
        else if (enemyType == "minion")
        {
            StartCoroutine(GetComponent<MinionEnemy>().GotHit(atkKnockback));
        }
        else if (enemyType == "queen")
        {
            StartCoroutine(GetComponent<QueenEnemy>().GotHit(atkKnockback));
        }

        //Debug.Log("Damage: " + dmg + " , HP: " + HP + "/" + maxHP);

        if (isMainEnemy)
        {
            StartCoroutine(BattleManager.instance.PlayerAttackUI());
            StartCoroutine(BattleManager.instance.UpdateEnemyHP((HP + dmg) / maxHP, HP / maxHP));

            if (HP <= 0)
            {
                GameOver.instance.quotaState = nm.currentQuota;
                pm.EndBattle(nm.currentQuota, nm.maxQuota); //update game state
            }
        }
        
        if (HP <= 0)
        {
            if (PlayerBattle.Instance.lockedOn == true) // TO DO : update to consider meteor
            {
                PlayerBattle.Instance.lockedOn = false;
                PlayerBattle.Instance.LockOn();

                Debug.Log("lockingOff");
            }
            
            if (enemyType == "boss")
            {
                GetComponent<BossEnemy>().EndBattle();
                GetComponent<EnemyIntermission>().enabled = true;
            }

            else if (enemyType == "minion")
            {
                BattleManager.instance.RemoveFromEnemyList(this.gameObject);
                Debug.Log("removing");
                GetComponent<MinionEnemy>().Death();
            }
            
            else if (enemyType == "queen")
            {
                GetComponent<QueenEnemy>().EndBattle();
            }
        }
    }

    public void AttackParried()
    {
        if (enemyType == "boss")
        {
            GetComponent<BossEnemy>().attackParried = true;
        }
        else if (enemyType == "minion")
        {
            GetComponent<MinionEnemy>().attackParried = true;
        }
        else if (enemyType == "queen")
        {
            GetComponent<QueenEnemy>().attackParried = true;
        }
    }

    public float DistanceCheck(Vector3 obj)
    {
        Vector3 distanceVector = obj - this.transform.position;
        float distance = distanceVector.magnitude;

        return distance;
    }

    public void StartBattle()
    {
        HP = maxHP;
        StartCoroutine(BattleManager.instance.UpdateEnemyHP(0, 1));

        if (enemyType == "queen" || enemyType == "boss")
        {
            BattleManager.instance.SetEnemySprite(enemyType);
        }

        if (!BattleManager.instance.activeEnemy.Contains(this.gameObject))
        {
            BattleManager.instance.AddToEnemyList(this.gameObject);
        }
    }
}

