using UnityEngine;

public class AttackBox : MonoBehaviour
{
    public float damage = 30;
    public float movement = 1;
    public float knockback = 10;

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
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();

            float damageCalc = damage;
            float knockbackCalc = knockback;

            /* GameObject dmgNumber = Instantiate(other.GetComponent<Enemy>().dmgNumbers, other.transform.position, Quaternion.identity);
            dmgNumber.GetComponentInChildren<TextMesh>().text = damageCalc.ToString(); */

            enemy.HitByAttack(damageCalc, knockbackCalc);
            //hit audio;

            //HitBloom.gameObject.GetComponent<HitBloom>().hitCheck = true;
        }
    }
}
