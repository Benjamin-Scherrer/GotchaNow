using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
    public static VfxManager instance;
    private Material skybox;
    private float skyboxRotation;
    public float skyboxRotationSpeed;
    [Header("Default Skybox Settings")]
    public Color defaultTint;
    public float defaultExposure;

    void Awake()
    {
        instance = this;    
    }

    void Start()
    {
        skybox = RenderSettings.skybox;
        skyboxRotation = 0;

        skybox.SetFloat("_Rotation", skyboxRotation);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        skyboxRotation += skyboxRotationSpeed * Time.fixedDeltaTime;
        if (skyboxRotation > 360) skyboxRotation -= 360;

        skybox.SetFloat("_Rotation", skyboxRotation);
        //Debug.Log
    }

    public IEnumerator SpecialAttackFlash(float flashExposure, float attackTime)
    {
        float timer = 0;
        float currentExposure = skybox.GetFloat("_Exposure");

        while (timer < attackTime/2)
        {
            timer += Time.fixedDeltaTime;
            
            skybox.SetFloat("_Exposure", Mathf.Lerp(currentExposure, flashExposure, timer/(attackTime/4)));
            yield return new WaitForFixedUpdate();
        }

        while (timer < attackTime)
        {
            timer += Time.fixedDeltaTime;
            
            skybox.SetFloat("_Exposure", Mathf.Lerp(flashExposure, currentExposure, timer/attackTime));
            yield return new WaitForFixedUpdate();
        }
    }
}
