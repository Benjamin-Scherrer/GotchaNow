using UnityEngine;

public class ArenaBorders : MonoBehaviour
{
    public Transform arenaCenter;
    public float pushStrength;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 pushVector = arenaCenter.position - collision.gameObject.transform.position;
            pushVector.y = 0;

            collision.gameObject.transform.position += pushVector * pushStrength * Time.deltaTime;
        }
    }
}
