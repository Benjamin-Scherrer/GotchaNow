using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
    public GameObject player;
    public GameObject target;
    private Vector3 distance;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distance = target.transform.position - player.transform.position;
        transform.position = player.transform.position + distance/2;
    }
}
