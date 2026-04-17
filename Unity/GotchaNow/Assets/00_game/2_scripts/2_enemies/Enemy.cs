using UnityEngine;

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
    public bool alreadyDead = false;

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
        if (BattleManager.instance != null)
        {
            BattleManager.instance.RemoveFromEnemyList(this.gameObject);
        }
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

        if (isMainEnemy)
        {
            StartCoroutine(BattleManager.instance.PlayerAttackUI());
            StartCoroutine(BattleManager.instance.UpdateEnemyHP((HP + dmg) / maxHP, HP / maxHP));
        }

        if (HP <= 0 && !alreadyDead)
        {
            alreadyDead = true;

            if (PlayerBattle.Instance.lockedOn == true) // TO DO : update to consider meteor
            {
                PlayerBattle.Instance.lockedOn = false;
                PlayerBattle.Instance.LockOn();

                Debug.Log("lockingOff");
            }
            
            if (enemyType == "minion")
            {
                Debug.Log("removing");
                BattleManager.instance.RemoveFromEnemyList(this.gameObject);
                GetComponent<MinionEnemy>().Death();
            }

            if (isMainEnemy)
            {
                if (enemyType == "boss")
                {
                    GetComponent<BossEnemy>().EndBattle();
                }
                else if (enemyType == "queen")
                {
                    GetComponent<QueenEnemy>().EndBattle();
                }

                GameOver.instance.quotaState = nm.currentQuota;
                StartCoroutine(ProgressionManager.instance.EndBattleRoutine(nm.currentQuota, nm.maxQuota)); //update game state
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

        alreadyDead = false;

        if (enemyType == "queen" || enemyType == "boss")
        {
            BattleManager.instance.SetEnemySprite(enemyType);
        }

        if (enemyType == "boss")
        {
            GetComponent<BossEnemy>().StartBattle();
        }

        if (!BattleManager.instance.activeEnemy.Contains(this.gameObject))
        {
            BattleManager.instance.AddToEnemyList(this.gameObject);
        }
    }
}

