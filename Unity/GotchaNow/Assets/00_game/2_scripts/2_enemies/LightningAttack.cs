using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class LightningAttack : MonoBehaviour
{
    public GameObject hitbox;
    //private CapsuleCollider hitboxCollider;
    public float hitboxSpawnDelay;
    public float hitboxActiveTime;
    public float lifeTime;
    public Vector3 hitboxMaxSize;
    public float lightningSize;
    public ParticleSystem ps;
    void Start()
    {
        //hitboxCollider = hitbox.GetComponent<CapsuleCollider>();
        hitbox.SetActive(false);
        
        var main = ps.main;
        main.startSizeX = lightningSize;

        StartCoroutine(LightningStrike());
    }

    private IEnumerator LightningStrike()
    {
        float timer = 0;

        while (timer < hitboxSpawnDelay)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        hitbox.SetActive(true);

        while (timer < hitboxActiveTime)
        {
            timer += Time.fixedDeltaTime;

            hitbox.transform.localScale = Vector3.Lerp(hitbox.transform.localScale, hitboxMaxSize, (timer-hitboxSpawnDelay)/(hitboxActiveTime-hitboxSpawnDelay));

            yield return new WaitForFixedUpdate();
        }

        hitbox.SetActive(false);

        while (timer < lifeTime)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Destroy(this.gameObject);
    }
}
