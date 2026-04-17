using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class VfxManager : MonoBehaviour
{
    public static VfxManager instance;
    private Material skybox;
    private float skyboxRotation;
    public float skyboxRotationSpeed;
    [Header("Default Skybox Settings")]
    public Color defaultTint;
    public float defaultExposure;
    public Volume fxVolume;

    void Awake()
    {
        instance = this;    
    }

    void Start()
    {
        skybox = RenderSettings.skybox;
        skyboxRotation = 0;

        skybox.SetFloat("_Rotation", skyboxRotation);

        StartCoroutine(SetDOF(1000,0,false));
        StartCoroutine(SetBloom(0.5f,0));
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

    public IEnumerator SetDOF(float newValue, float time, bool enable)
    {
        float timer = 0;

        fxVolume.profile.TryGet(out DepthOfField dof);

        if (enable) dof.active = true;

        float currentValue = dof.gaussianEnd.value;
        //Debug.Log(currentValue);

        while (timer < time)
        {
            timer += Time.unscaledTime;
            dof.gaussianEnd.value = Mathf.Lerp(currentValue, newValue, timer/time);

            yield return new WaitForSecondsRealtime(0.16f);
        }

        dof.gaussianEnd.value = newValue;
        if (!enable) dof.active = false;
    }

    public IEnumerator SetBloom(float newValue, float time)
    {
        float timer = 0;

        fxVolume.profile.TryGet(out Bloom bloom);

        float currentValue = bloom.intensity.value;

        while (timer < time)
        {
            timer += Time.unscaledTime;
            bloom.intensity.value = Mathf.Lerp(currentValue, newValue, timer/time);

            yield return new WaitForSecondsRealtime(0.016f);
        }

        bloom.intensity.value = newValue;
    }
}
