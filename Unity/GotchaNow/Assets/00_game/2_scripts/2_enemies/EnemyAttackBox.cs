using System.Collections;
//using System.Numerics;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyAttackBox : MonoBehaviour
{
    public Enemy enemy;
    public AttackScript attackScript;
    public float damage = 30;
    public float movement = 1;
    public float knockback = 10;
    public bool isComboAtk = false;
    private bool attackBlocked = false;
    private bool attackParried = false;
    public VisualEffect hitGlow;

    //private GameObject HitBloom;

    void OnEnable()
    {
        attackParried = false;
        attackBlocked = false;

        /* if (enemy != null)
        {
            enemy = GetComponentInParent<Enemy>();
        } */
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
            Vector3 attackDir = new Vector3(other.transform.position.x, 0, other.transform.position.z) - new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z);

            if (attackParried)
            {
                pb.ParrySuccessful();
                enemy.AttackParried();
                attackParried = false;
            }
            else
            {
                if (attackBlocked)
                {
                    damageCalc = damage / 4;
                    //knockbackCalc = knockback * 0.7f;
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

                //hit audio;
            }
        }
    }

    public IEnumerator ComboAttack(float duration)
    {
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        gameObject.SetActive(false);
    }
}
