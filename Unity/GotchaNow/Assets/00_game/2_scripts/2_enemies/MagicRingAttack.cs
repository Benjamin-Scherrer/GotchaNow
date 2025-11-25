using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using UnityEngine.Splines.Interpolators;
using System;

public class MagicRingAttack : MonoBehaviour
{
    public float attackDelay;
    public float lifeTime;
    public float despawnTime;
    public GameObject lightningStrike;
    public ParticleSystem ps1;
    public ParticleSystem ps2;
    public ParticleSystem ps3;


    void Start()
    {
        //set lifetime of particle systems
        var main1 = ps1.main;
        var main2 = ps2.main;
        var main3 = ps3.main;
        
        main1.startLifetime = lifeTime;
        main2.startLifetime = lifeTime;
        main3.startLifetime = lifeTime;

        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit rch;
        if (Physics.Raycast(ray, out rch, Mathf.Infinity))
        {
            transform.position = new Vector3 (transform.position.x, rch.point.y, transform.position.z);
        }

        StartCoroutine(MagicRingRoutine());
    }

    private IEnumerator MagicRingRoutine()
    {
        float timer = 0;

        while (timer < attackDelay)
        {
            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        Instantiate(lightningStrike, transform.position, Quaternion.identity);

        while (timer < lifeTime)
        {
            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        Destroy(this.gameObject);
    }
}
