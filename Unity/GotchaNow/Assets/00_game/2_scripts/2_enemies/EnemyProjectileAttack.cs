using UnityEngine;
using System.Collections;

public class EnemyProjectileAttack : MonoBehaviour
{
    //public Enemy enemy;
    public float damage = 20;
    public float knockback = 10;
    private bool attackBlocked = false;
    private bool attackParried = false;
    public bool isComboAtk = false;
    private EnemyMeteorShot enemyMeteorShot;
    public bool isMeteorShot = false;

    //private GameObject HitBloom;

    void OnEnable()
    {
        attackParried = false;
        attackBlocked = false;

        if (isMeteorShot)
        {
            enemyMeteorShot = GetComponent<EnemyMeteorShot>();
        }   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBlock"))
        {
            if (other.GetComponentInParent<BlockScript>().parryActive)
            {
                attackParried = true;
            }
            else
            {
                attackBlocked = true;
            }
        }

        if (other.CompareTag("Player"))
        {
            StartCoroutine(PlayerHit(other));
        }
    }
    
    private IEnumerator PlayerHit(Collider other)
    {
        yield return null; //delay for 1 frame so block/parry is always checked first

        {
            PlayerBattle pb = other.GetComponent<PlayerBattle>();

            float damageCalc = damage;
            float knockbackCalc = knockback;
            Vector3 attackDir = new Vector3(other.transform.position.x, 0, other.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z);

            if (attackParried)
            {
                pb.ParrySuccessful();
                attackParried = false;
            }
            else
            {
                if (attackBlocked)
                {
                    damageCalc = damage / 4;
                    knockbackCalc = knockback * 0.7f;
                    attackBlocked = false;
                    Debug.Log("block");
                }

                pb.HitByAttack(damageCalc, knockbackCalc, attackDir, isComboAtk);
            }

            if (isMeteorShot)
            {
                enemyMeteorShot.Hit();
            }
        }
    }
}
