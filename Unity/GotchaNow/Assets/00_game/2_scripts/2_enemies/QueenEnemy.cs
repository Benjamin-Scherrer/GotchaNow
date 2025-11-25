using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenEnemy : MonoBehaviour
{
    private Rigidbody rb;
    private Enemy enemy;
    private PlayerBattle pb;
    public GameObject model;
    public Animator animator;
    private bool actionInProgress = false;
    private float distance;
    private string behaviorPhase = null;
    private bool readyForAttack = false;
    private bool strafeActive = false;
    private bool approachActive = false;
    private List<string> availableAttacks = new List<string>();
    private List<string> attackChoice = new List<string>();

    [Header("Basic Attributes")]
    public float walkSpeed = 3;
    public float strafeSpeed = 2;
    public float turnSpeed = 0.3f;
    public GameObject attack1;
    public float attack1Duration;
    public float attack1EndingLag;
    [Header("Meteor Shot Attack")]
    public GameObject meteorShot;
    public float meteorShotRange;
    public float meteorShotStartupTime;
    public float meteorShotDuration;
    public float meteorShotMotionTime;
    public float meteorShotSpeed;
    public float meteorShotEndingLag;
    [Header("Magic Ring Attack")]
    public GameObject magicRing;
    public float magicRingRange;
    public float magicRingStartupTime;
    public float magicRingDuration;
    public float magicRingMotionTime;
    public float magicRingSpeed;
    public float magicRingEndingLag;
    [Header("Parry etc")]
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
        //
    }
    
    void Start()
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

        StartCoroutine(MagicRing());
        actionInProgress = true;
    }

    //MOVEMENT FUNCTIONS
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

        //animator.SetFloat("motionFwd", 1);
        rb.MovePosition(rb.position + pVector.normalized * walkSpeed * speedMult * Time.fixedDeltaTime);
    }
    
    private void AvoidPlayer(float speedMult)
    {
        Vector3 pVector = pb.gameObject.transform.position - rb.position;
        pVector.y = 0;

        //animator.SetFloat("motionFwd", -1);
        rb.MovePosition(rb.position - pVector.normalized * walkSpeed * speedMult * Time.fixedDeltaTime);
    }

    private void StrafeAroundPlayer(string dir, float speedMult)
    {
        if (dir == "r")
        {
            //animator.SetFloat("motionSide", 1);
            rb.MovePosition(rb.position + transform.right * strafeSpeed * speedMult * Time.fixedDeltaTime);
        }

        if (dir == "l")
        {
            //animator.SetFloat("motionSide", -1);
            rb.MovePosition(rb.position - transform.right * strafeSpeed * speedMult * Time.fixedDeltaTime);
        }
    }

    //MOVEMENT COROUTINES
    private IEnumerator ApproachAction(float actionTime)
    {
        float timer = 0;

        approachActive = true;
        //animator.SetBool("Walking", true);

        if (behaviorPhase == "start" || behaviorPhase == "phase1") readyForAttack = false;
        else if (behaviorPhase == "phase2") readyForAttack = true;

        //WIP: set strafe direction
        int rndm = UnityEngine.Random.Range(0, 2);
        /* string strafeDir;
        if (rndm == 0) strafeDir = "l";
        else strafeDir = "r"; */

        while (approachActive)
        {
            while (timer < actionTime)
            {
                timer += Time.fixedDeltaTime;

                if (behaviorPhase == "start" && (timer > actionTime/2))
                {
                    readyForAttack = true;
                }
                else if (behaviorPhase == "phase1" && (timer > actionTime/4))
                {
                    readyForAttack = true;
                }

                LookAtPlayer(0.75f);
                WalkTowardsPlayer(1);
                //StrafeAroundPlayer(strafeDir, 1);

                yield return new WaitForFixedUpdate();
            }

            /* if (behaviorPhase == "start")
            {
                int rndmMove = UnityEngine.Random.Range(0,4);
                if (rndmMove == 0 || rndmMove == 1) StartCoroutine(ApproachAction(4));
                else if (rndmMove == 2) StartCoroutine(StrafeAction(6));
                else if (rndmMove == 3) StartCoroutine(ShoulderBash());
            }
            else if (behaviorPhase == "phase1")
            {
                int rndmMove = UnityEngine.Random.Range(0,3);
                if (rndmMove == 0) StartCoroutine(ApproachAction(4));
                else if (rndmMove == 1) StartCoroutine(StrafeAction(5));
                else if (rndmMove == 2) StartCoroutine(ShoulderBash());
            }
            else if (behaviorPhase == "phase2")
            {
                int rndmMove = UnityEngine.Random.Range(0,4);
                if (rndmMove == 0) StartCoroutine(ApproachAction(3));
                else if (rndmMove == 1) StartCoroutine(StrafeAction(5));
                else if (rndmMove == 2 || rndmMove == 3) StartCoroutine(ShoulderBash());
            } */

            //actionInProgress = false;
            approachActive = false;
        }
    }

    private IEnumerator StrafeAction(float actionTime)
    {
        float timer = 0;

        strafeActive = true;
        //animator.SetBool("Walking", true);

        if (behaviorPhase == "start" || behaviorPhase == "phase1") readyForAttack = false;
        else if (behaviorPhase == "phase2") readyForAttack = true;

        //WIP: set strafe direction
        int rndm = UnityEngine.Random.Range(0, 2);
        string strafeDir;
        if (rndm == 0) strafeDir = "l";
        else strafeDir = "r";

        while (strafeActive)
        {
            while (timer < actionTime)
            {
                timer += Time.fixedDeltaTime;

                if (readyForAttack == false && behaviorPhase == "start" && (timer > actionTime/2))
                {
                    readyForAttack = true;
                }
                else if (readyForAttack == false && behaviorPhase == "phase1" && (timer > actionTime/4))
                {
                    readyForAttack = true;
                }

                LookAtPlayer(0.75f);
                WalkTowardsPlayer(1);
                StrafeAroundPlayer(strafeDir, 1);

                yield return new WaitForFixedUpdate();
            }

            /* if (behaviorPhase == "start")
            {
                int rndmMove = UnityEngine.Random.Range(0,2);
                if (rndmMove == 0) StartCoroutine(ApproachAction(5));
                else if (rndmMove == 1) StartCoroutine(ShoulderBash());
            }
            else if (behaviorPhase == "phase1")
            {
                int rndmMove = UnityEngine.Random.Range(0,2);
                if (rndmMove == 0) StartCoroutine(ApproachAction(5));
                else if (rndmMove == 1) StartCoroutine(ShoulderBash());
            }
            else if (behaviorPhase == "phase2")
            {
                int rndmMove = UnityEngine.Random.Range(0,3);
                if (rndmMove == 0) StartCoroutine(ApproachAction(5));
                else if (rndmMove == 1 || rndmMove == 2) StartCoroutine(ShoulderBash());
            } */

            actionInProgress = false;
            strafeActive = false;
        }        
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

    private IEnumerator MeteorShot()
    {
        //AttackScript atkScript = MeteorShotAttack.GetComponent<AttackScript>();

        actionInProgress = true;

        float atkTimer = 0;

        while (atkTimer < meteorShotStartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(0.33f);

            yield return new WaitForFixedUpdate();
        }

        //atkScript.StartAttack(); //enable hitbox
        Instantiate(meteorShot, transform.position + transform.forward, transform.rotation);
        atkTimer = 0;

        while (atkTimer < meteorShotDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < meteorShotMotionTime)
            {   
                //LookAtPlayer(0.2f);
            }

            yield return new WaitForFixedUpdate();
        }

        //atkScript.EndAttack();

        yield return new WaitForSeconds(meteorShotEndingLag); //ending lag

        actionInProgress = false;
    }

    private IEnumerator MagicRing()
    {
        //AttackScript atkScript = MeteorShotAttack.GetComponent<AttackScript>();

        actionInProgress = true;

        float atkTimer = 0;

        while (atkTimer < magicRingStartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(0.33f);

            yield return new WaitForFixedUpdate();
        }

        //atkScript.StartAttack(); //enable hitbox
        Instantiate(magicRing, transform.position + 5* transform.forward, transform.rotation);
        atkTimer = 0;

        while (atkTimer < magicRingDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < magicRingMotionTime)
            {   
                //LookAtPlayer(0.2f);
            }

            yield return new WaitForFixedUpdate();
        }

        //atkScript.EndAttack();

        yield return new WaitForSeconds(magicRingEndingLag); //ending lag

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
        this.enabled = false;
    }
}
