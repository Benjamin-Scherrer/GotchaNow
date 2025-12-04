using FMODUnity;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using UnityEngine.Splines.Interpolators;
using System;
//using System.Numerics;

public class MagicRingAttack : MonoBehaviour
{
    public float attackDelay;
    public float lifeTime;
    //public float despawnTime;
    public GameObject lightningStrike;
    public ParticleSystem ps1;
    public ParticleSystem ps2;
    public ParticleSystem ps3;
    public Transform trackingTarget;
    public bool spawner = false;
    public GameObject minion;
    public EventReference magicRingSFX;

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

        RuntimeManager.PlayOneShot(magicRingSFX, transform.position);

        if (spawner)
        {
            StartCoroutine(SpawnMinion());
        }
        else
        {
            StartCoroutine(MagicRingRoutine());
        }
    }

    private IEnumerator MagicRingRoutine()
    {
        float timer = 0;

        while (timer < attackDelay)
        {
            timer += Time.fixedDeltaTime;

            if (trackingTarget != null)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(trackingTarget.position.x, transform.position.y, trackingTarget.position.z), 0.9f);
            }

            yield return new WaitForFixedUpdate();
        }

        Instantiate(lightningStrike, transform.position, Quaternion.identity, this.transform);

        while (timer < lifeTime)
        {
            if (trackingTarget != null && timer < attackDelay + 0.7f)
            {
                //Debug.Log("still tracking");
                transform.position = Vector3.Lerp(transform.position, new Vector3(trackingTarget.position.x, transform.position.y, trackingTarget.position.z), 0.03f);
            }

            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        Destroy(this.gameObject);
    }

    private IEnumerator SpawnMinion()
    {
        float timer = 0;

        while (timer < attackDelay)
        {
            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        Debug.Log("spawning minion");
        Instantiate(minion, transform.position + transform.up, Quaternion.identity);

        while (timer < lifeTime)
        {
            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        Destroy(this.gameObject);
    }
}
