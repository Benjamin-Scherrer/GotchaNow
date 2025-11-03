using System.Collections;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public GameObject hitbox;
    public GameObject hitbox2;
    public string attackType;
    public float windupTime;
    public float windup2Time;
    public float endTime;
    public float hitboxDmg;
    public float hitbox2Dmg;
    public string attackType;
    public float windupTime;

    public void StartAttack()
    {
        if (attackType == "playerAttack")
        {
            StartCoroutine(PlayerAttack());
        }
        /* else if (attackType == "playerHeavyAttack")
        {
            StartCoroutine(PlayerHeavyAttack());
        } */
        else
        {
        if (attackType == "playerAttack")
        {
            StartCoroutine(PlayerAttack());
        }
        else
        {
                hitbox.SetActive(true);
        }
        }
    }

    

    public void EndAttack()
    {
        hitbox.SetActive(false);
    }

    private IEnumerator PlayerAttack()
    {
        float timer = 0;

        hitbox.GetComponent<AttackBox>().damage = hitboxDmg;

        while (timer < windupTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        hitbox.SetActive(true);

        while (timer < endTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        hitbox.SetActive(false);
    }

    /* private IEnumerator PlayerHeavyAttack()
    {
        float timer = 0;

        hitbox.GetComponent<AttackBox>().damage = hitboxDmg;
        hitbox2.GetComponent<AttackBox>().damage = hitbox2Dmg;

        while (timer < windupTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        hitbox.SetActive(true);

        while (timer < windup2Time)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        hitbox2.SetActive(true);

        while (timer < endTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        hitbox.SetActive(false);
        hitbox2.SetActive(false);
    } */

    private IEnumerator PlayerAttack()
    {
        float timer = 0;

        while (timer < windupTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        hitbox.SetActive(true);
    }
}
