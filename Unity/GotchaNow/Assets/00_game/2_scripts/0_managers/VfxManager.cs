using UnityEngine;

public class VfxManager : MonoBehaviour
{
    private Material skybox;
    private float skyboxRotation;
    public float skyboxRotationSpeed;
    [Header("Default Skybox Settings")]
    public Color defaultTint;
    public float defaultExposure;

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
}
