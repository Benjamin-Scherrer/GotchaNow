using System.Collections;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class EnemyMeteorShot : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public float lifeTime = 3;
    private Transform playerTransform;
    private Vector3 direction;
    public GameObject explosion;
    private EnemyProjectileAttack enemyProjectileAttack;
    
    void OnEnable()
    {
        enemyProjectileAttack = GetComponent<EnemyProjectileAttack>();
        playerTransform = PlayerBattle.Instance.gameObject.transform;

        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        float timer = 0;
        
        while (timer < lifeTime)
        {
            timer += Time.fixedDeltaTime;

            float frontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(playerTransform.position));
            if (frontCheck > 0)
            {
                direction = (playerTransform.position - transform.position).normalized;
                direction.y = 0;
            }

            transform.position = transform.position + speed * Time.fixedDeltaTime * Vector3.Lerp(transform.forward,direction,turnSpeed).normalized;

            yield return new WaitForFixedUpdate();
        } 

        StartCoroutine(Dissolve());        
    }

    public void Hit()
    {
        Instantiate(explosion,transform.position,Quaternion.identity);
        Destroy(this.gameObject);
    }

    private IEnumerator Dissolve()
    {
        float timer = 0;
        float dissolveTime = 0.3f;

        enemyProjectileAttack.enabled = false;

        while (timer < dissolveTime)
        {
            timer += Time.fixedDeltaTime;
           
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, timer/dissolveTime);

            yield return new WaitForFixedUpdate();
        }

        Destroy(this.gameObject);
    }
}
