using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class EnemyAttackBox : MonoBehaviour
{
    public Enemy enemy;
    public AttackScript attackScript;
    public float damage = 30;
    public float movement = 1;
    public float knockback = 10;
    private bool attackBlocked = false;
    private bool attackParried = false;

    //private GameObject HitBloom;

    void Start()
    {

    }

    void OnEnable()
    {
        //attack audio
        //HitBloom = GameObject.Find("VFXBloomWhite");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBlock")
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
        
        if (other.tag == "Player")
        {
            PlayerBattle pb = other.GetComponent<PlayerBattle>();

            float damageCalc = damage;
            float knockbackCalc = knockback;
            Vector3 attackDir = new Vector3(other.transform.position.x, 0, other.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z);

            if (attackParried)
            {
                pb.ParrySuccessful();
                enemy.AttackParried();
                attackParried = false;

                Debug.Log("parry");

                //attackScript.EndAttack();
            }
            else
            {
                if (attackBlocked)
                {
                    damageCalc = damage / 4;
                    knockbackCalc = knockback / 2;
                    attackBlocked = false;
                    Debug.Log("block");
                }

                /* GameObject dmgNumber = Instantiate(other.GetComponent<Enemy>().dmgNumbers, other.transform.position, Quaternion.identity);
                dmgNumber.GetComponentInChildren<TextMesh>().text = damageCalc.ToString(); */

                pb.HitByAttack(damageCalc, knockbackCalc, attackDir);

                //hit audio;
                //HitBloom.gameObject.GetComponent<HitBloom>().hitCheck = true;
            }
        }
    }
}
