using System.Collections;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    private Rigidbody rb;
    private Enemy enemy;
    private PlayerBattle pb;
    public GameObject model;
    private bool actionInProgress = false;
    private float distance;
    public float walkSpeed = 3;
    public float strafeSpeed = 2;
    public float turnSpeed = 0.3f;
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
        /* //test
        LookAtPlayer(1);
        WalkTowardsPlayer(1);
        StrafeAroundPlayer("r", 1); */
        
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
        Debug.Log("distance to player: " + distance);

        if (distance <= 3)
        {
            StartCoroutine(Attack1());
        }
        else
        {
            //approach player

            StartCoroutine(ApproachPlayer(1));
        }

        actionInProgress = true;
    }

    private void LookAtPlayer(float speedMult)
    {
        Vector3 pVector = pb.gameObject.transform.position - rb.position;
        pVector.y = 0;

        transform.LookAt(Vector3.Lerp(rb.position + transform.forward, rb.position + pVector.normalized, turnSpeed * speedMult));
    }

    private void WalkTowardsPlayer(float speedMult)
    {
        Vector3 pVector = pb.gameObject.transform.position - rb.position;
        pVector.y = 0;

        rb.MovePosition(rb.position + pVector.normalized * walkSpeed * speedMult * Time.fixedDeltaTime);
    }
    
    private void AvoidPlayer(float speedMult)
    {
        Vector3 pVector = pb.gameObject.transform.position - rb.position;
        pVector.y = 0;

        rb.MovePosition(rb.position - pVector.normalized * walkSpeed * speedMult * Time.fixedDeltaTime);
    }

    private void StrafeAroundPlayer(string dir, float speedMult)
    {
        /* Vector3 pVector = pb.gameObject.transform.position - transform.position;
        pVector.y = 0; */

        if (dir == "r")
        {
            rb.MovePosition(rb.position + transform.right * strafeSpeed * speedMult * Time.fixedDeltaTime);
        }

        if (dir == "l")
        {
            rb.MovePosition(rb.position - transform.right * strafeSpeed * speedMult * Time.fixedDeltaTime);
        }
    }

    private IEnumerator ApproachPlayer(float actionTime)
    {
        float timer = 0;

        //WIP: set strafe direction
        int rndm = Random.Range(0, 2);
        string strafeDir;
        if (rndm == 0) strafeDir = "l";
        else strafeDir = "r";

        while (timer < actionTime)
        {
            timer += Time.fixedDeltaTime;

            LookAtPlayer(1);
            WalkTowardsPlayer(1);
            StrafeAroundPlayer(strafeDir, 1);

            yield return new WaitForFixedUpdate();
        }

        actionInProgress = false;
    }

    //test attack
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

    //when attack is parried by player
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
                rb.MovePosition(transform.position + parryDir.normalized * knockback * Time.fixedDeltaTime);
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
