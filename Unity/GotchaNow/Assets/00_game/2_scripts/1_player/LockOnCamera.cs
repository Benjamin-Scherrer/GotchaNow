using UnityEngine;
using Unity.Cinemachine;

public class LockOnCamera : MonoBehaviour
{
    public GameObject player;
    public GameObject target;
    private Vector3 distance;
    private Vector3 camPos;
    public GameObject freeCam;
    [HideInInspector] public bool isActive = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distance = target.transform.position - player.transform.position;

        camPos = player.transform.position - new Vector3(distance.normalized.x, 0f, distance.normalized.z) * 8 + new Vector3(0, 2f, 0);
        transform.position = Vector3.Lerp(transform.position, camPos, 0.1f);

        if (isActive)
        {
            //freeCam.GetComponent<CinemachineOrbitalFollow>().ForceCameraPosition(transform.position,transform.rotation);
        }
    }
}
