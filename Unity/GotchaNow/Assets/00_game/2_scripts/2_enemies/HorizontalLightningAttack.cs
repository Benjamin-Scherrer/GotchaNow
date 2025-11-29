using System.Collections;
using UnityEngine;

public class HorizontalLightningAttack : MonoBehaviour
{
    public GameObject hitbox;
    public GameObject strike;
    public GameObject flare;
    public float lifetime = 2f;
    public float hitboxSpawnDelay = 0.5f;
    public float despawnTime = 0.2f;

    void OnEnable()
    {
        hitbox.SetActive(false);
        strike.SetActive(true);
        flare.SetActive(true);

        StartCoroutine(Fire());
    }

    // Update is called once per frame
    public IEnumerator Fire()
    {
        float timer = 0;

        while (timer < hitboxSpawnDelay)
        {
            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        hitbox.SetActive(true);

        while (timer < lifetime)
        {
            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(EndAttack());
    }


    public IEnumerator EndAttack()
    {
        hitbox.SetActive(false);
        strike.SetActive(false);

        float timer = 0;
        
        while (timer < despawnTime)
        {
            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        gameObject.SetActive(false);
    }
}
