using UnityEngine;
using UnityEngine.VFX;
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
    public VisualEffect hitGlow;

    //private GameObject HitBloom;

    void OnEnable()
    {
        attackParried = false;
        attackBlocked = false;

        if (isMeteorShot)
        {
            enemyMeteorShot = GetComponent<EnemyMeteorShot>();
        }

        ProgressionManager.instance.EndBattleEvent.AddListener(EndBattle);
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
                    
                    VisualEffect smallGlow = Instantiate(hitGlow, transform.position, Quaternion.identity);
                    smallGlow.transform.localScale = Vector3.one * 0.5f;
                }
                else
                {
                    Instantiate(hitGlow, transform.position, Quaternion.identity);
                }

                pb.HitByAttack(damageCalc, knockbackCalc, attackDir, isComboAtk);
            }

            if (isMeteorShot)
            {
                enemyMeteorShot.Hit();
            }
        }
    }

    void EndBattle() //triggered through event
    {
        Destroy(this.gameObject);
    }
}
