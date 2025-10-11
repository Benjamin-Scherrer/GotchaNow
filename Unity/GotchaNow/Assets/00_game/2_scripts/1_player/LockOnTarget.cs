using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
    public GameObject player;
    public GameObject targetEnemy;
    private Vector3 distance;
    public float lerpSpeed = 0.2f;

    void Start()
    {
        targetEnemy = null;
    }

    void FixedUpdate()
    {
        if (targetEnemy != null)
        {
            //target for lock-on cam, situated in the middle between player and locked on enemy
            
            distance = targetEnemy.transform.position - player.transform.position;
            transform.position = Vector3.Lerp(transform.position, player.transform.position + distance/2,lerpSpeed);
        }
    }
}
