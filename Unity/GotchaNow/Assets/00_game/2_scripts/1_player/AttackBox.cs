using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using UnityEngine.VFX;

public class AttackBox : MonoBehaviour
{
    public float baseDmg = 30;
    public float damage = 30;
    public float duration = 1;
    public float movement = 1;
    public float knockback = 10;
    public float buffMult = 1.5f;
    public VisualEffect hitGlow;

    //private GameObject HitBloom;

    void OnEnable()
    {
        StartCoroutine(Duration(duration));

        if (PlayerBattle.Instance.buffActive)
        {
            Debug.Log("buffed atk");
            damage = baseDmg * buffMult;
        }
        else if (!PlayerBattle.Instance.buffActive)
        {
            damage = baseDmg;
        }
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

            Instantiate(hitGlow, transform.position, Quaternion.identity);

            //hit audio;

            //HitBloom.gameObject.GetComponent<HitBloom>().hitCheck = true;
        }
    }

    public IEnumerator Duration(float deactTime)
    {
        float timer = 0;

        while (timer <= deactTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
