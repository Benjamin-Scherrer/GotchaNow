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
    public BattleManager bm;
    public ProgressionManager pm;
    public NotificationManager nm;
    public bool isLockOnTarget = false;
    public bool isMainEnemy = false;

    [HideInInspector] public bool hit = false;

    //public GameObject dmgNumbers;
    //private GameObject HitBloom;

    private void Start()
    {
        //bm = BattleManager.instance;

        //HitBloom = GameObject.Find("VFXBloomWhite");
    }

    private void OnEnable()
    {
        bm = BattleManager.instance;
        pm = ProgressionManager.instance;
        nm = NotificationManager.instance;

        bm.AddToEnemyList(this.gameObject);
    }

    private void OnDisable()
    {
        bm.RemoveFromEnemyList(this.gameObject);
    }

    private void FixedUpdate()
    {
        if (HP <= 0) //check if alive
        {
            //HitBloom.gameObject.GetComponent<HitBloom>().killCheck = true;

            //Destroy(gameObject);
        }
    }

    public void HitByAttack(float dmg, float atkKnockback)
    {
        hit = true;
        HP -= dmg;
        knockback = atkKnockback;

        //Debug.Log("Damage: " + dmg + " , HP: " + HP + "/" + maxHP);

        if (isMainEnemy)
        {
            StartCoroutine(bm.PlayerAttackUI());
            StartCoroutine(bm.UpdateEnemyHP((HP + dmg) / maxHP, HP / maxHP));

            if (HP <= 0)
            {
                pm.EndBattle(nm.currentQuota, nm.maxQuota); //update game state
            }
        }
        
        if (HP <= 0)
        {
            if (enemyType == "boss")
            {
                GetComponent<BossEnemy>().EndBattle();
                GetComponent<EnemyIntermission>().enabled = true;
            }
            else if (enemyType == "minion")
            {
                Destroy(this.gameObject);
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
        StartCoroutine(bm.UpdateEnemyHP(0, 1));
    }
}

