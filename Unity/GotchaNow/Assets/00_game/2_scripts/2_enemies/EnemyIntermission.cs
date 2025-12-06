using UnityEngine;

public class EnemyIntermission : MonoBehaviour
{
    private Rigidbody rb;
    private Enemy enemy;
    private PlayerIntermission pi;
    public GameObject model;
    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemy = GetComponent<Enemy>();
    }

    void OnEnable()
    {
        pi = PlayerIntermission.Instance;
        
        animator.SetTrigger("startIntermission");
    }

    public void EndIntermission()
    {
        animator.SetTrigger("endIntermission");
        
        GetComponent<BossEnemy>().enabled = true;
        this.enabled = false;
    }
}
