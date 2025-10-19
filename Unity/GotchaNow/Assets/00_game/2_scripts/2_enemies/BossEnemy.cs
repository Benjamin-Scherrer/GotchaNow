using System.Collections;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    private Rigidbody rb;
    private Enemy enemy;
    private PlayerBattle pb;
    public GameObject model;
    private bool actionInProgress = false;
    private float distance;
    public GameObject attack1;
    public float attack1Duration;
    public float attack1EndingLag;
    public bool attackParried = false;
    public float parryKnockback = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemy = GetComponent<Enemy>();
    }

    void OnEnable()
    {
        pb = PlayerBattle.Instance;   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!actionInProgress)
        {
            ChooseAction();
        }
    }

    private void Spawn()
    {

    }

    private void ChooseAction()
    {
        distance = enemy.DistanceCheck(pb.gameObject.transform.position);
        //Debug.Log("distance to player: " + distance);

        StartCoroutine(Attack1());
        actionInProgress = true;
    }

    private void WalkTowardsPlayer()
    {

    }

    private void StrafeAroundPlayer()
    {

    }
    
    private void AvoidPlayer()
    {
        
    }

    private IEnumerator Attack1()
    {
        AttackScript atkScript = attack1.GetComponent<AttackScript>();

        atkScript.StartAttack(); //enable hitbox

        float atkTimer = 0;

        while (atkTimer < attack1Duration)
        {
            atkTimer += Time.deltaTime;

            if (attackParried)
            {
                atkScript.EndAttack();
                StartCoroutine(AttackParried(2f, parryKnockback));
                yield break;
            }

            yield return null;
        }

        atkScript.EndAttack();

        yield return new WaitForSeconds(attack1EndingLag); //ending lag

        actionInProgress = false;
    }

    private IEnumerator AttackParried(float stunTime, float knockback)
    {
        float timer = 0;
        Vector3 parryDir = transform.position - pb.gameObject.transform.position;
        parryDir.y = 0;

        while (timer < stunTime)
        {
            timer += Time.deltaTime;

            if (knockback > 0)
            {
                rb.MovePosition(transform.position + parryDir.normalized * knockback * Time.deltaTime);
                knockback -= Time.deltaTime * 12;
            }

            yield return new WaitForFixedUpdate();
        }

        attackParried = false;
        actionInProgress = false;
    }
    
    public void EndBattle()
    {
        AttackScript atkScript = attack1.GetComponent<AttackScript>();
        atkScript.EndAttack();

        this.enabled = false;
    }
}
