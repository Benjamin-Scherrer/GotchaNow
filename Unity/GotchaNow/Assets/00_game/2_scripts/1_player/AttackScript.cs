using System.Collections;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public GameObject hitbox;
    public string attackType;
    public float windupTime;

    public void StartAttack()
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

    public void EndAttack()
    {
        hitbox.SetActive(false);
    }

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
