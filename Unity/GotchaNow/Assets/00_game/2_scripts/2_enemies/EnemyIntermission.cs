using UnityEngine;

public class EnemyIntermission : MonoBehaviour
{
    private Rigidbody rb;
    private Enemy enemy;
    private PlayerIntermission pi;
    public GameObject model;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemy = GetComponent<Enemy>();
    }

    void OnEnable()
    {
        pi = PlayerIntermission.Instance;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndIntermission()
    {
        GetComponent<BossEnemy>().enabled = true;
        this.enabled = false;
    }
}
