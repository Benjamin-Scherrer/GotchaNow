using FMODUnity;
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
    public EventReference hitSFX;

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

            enemy.HitByAttack(damageCalc, knockbackCalc);

            Instantiate(hitGlow, transform.position, Quaternion.identity);

            if(!hitSFX.IsNull)
            {
                RuntimeManager.PlayOneShot(hitSFX, transform.position);
            }
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
