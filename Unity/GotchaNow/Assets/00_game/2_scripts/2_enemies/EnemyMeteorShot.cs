using UnityEngine;

public class EnemyMeteorShot : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public float lifeTime = 3;
    private float timer = 0;
    private Transform playerTransform;
    private Vector3 direction;
    
    void OnEnable()
    {
        playerTransform = PlayerBattle.Instance.gameObject.transform;
        timer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > lifeTime)
        {
            Destroy(this.gameObject);
        }
        
        float frontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(playerTransform.position));
        if (frontCheck > 0)
        {
            direction = (playerTransform.position - transform.position).normalized;
            direction.y = 0;
        }

        transform.position = transform.position + speed * Time.fixedDeltaTime * Vector3.Lerp(transform.forward,direction,turnSpeed);
    }
}
