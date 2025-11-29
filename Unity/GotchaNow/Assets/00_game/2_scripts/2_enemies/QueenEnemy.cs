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
    public Vector3 meteorShotOffset;
    [Header("Magic Ring Attack")]
    public GameObject magicRing;
    public float magicRingRange;
    public float magicRingStartupTime;
    public float magicRingDuration;
    public float magicRingMotionTime;
    public float magicRingSpeed;
    public float magicRingEndingLag;
    [Header("Slash Combo 1 Attack")]
    public GameObject slashCombo1Attack;
    public float slashCombo1Range;
    public float slashCombo1StartupTime;
    public float slashCombo1Duration;
    public float slashCombo1MotionTime;
    public float slashCombo1Speed;
    public float slashCombo1EndingLag;
    [Header("Slash Combo 2 Attack")]
    public GameObject slashCombo2Attack;
    public float slashCombo2Range;
    public float slashCombo2StartupTime;
    public float slashCombo2Duration;
    public float slashCombo2MotionTime;
    public float slashCombo2Speed;
    public float slashCombo2EndingLag;
    [Header("Slash Combo 3 Attack")]
    public GameObject slashCombo3Attack;
    public float slashCombo3Range;
    public float slashCombo3StartupTime;
    public float slashCombo3Duration;
    public float slashCombo3MotionTime;
    public float slashCombo3Speed;
    public float slashCombo3EndingLag;
    [Header("Heavy Slash Attack")]
    public GameObject heavySlashAttack;
    public float heavySlashRange;
    public float heavySlashStartupTime;
    public float heavySlashDuration;
    public float heavySlashMotionTime;
    public float heavySlashSpeed;
    public float heavySlashEndingLag;
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
        
        //StartCoroutine(HeavySlash());
        StartCoroutine(SlashCombo1());
        //StartCoroutine(MeteorShot());
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

    private IEnumerator SlashCombo1()
    {
        AttackScript atkScript = slashCombo1Attack.GetComponent<AttackScript>();

        actionInProgress = true;
        readyForAttack = false;

        float atkTimer = 0;
        ResetWalkAnim();
        animator.SetTrigger("combo1");

        while (atkTimer < slashCombo1StartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(0.5f);

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        while (atkTimer < slashCombo1Duration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < slashCombo1MotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * slashCombo1Speed * Time.fixedDeltaTime);
            }

            if (attackParried)
            {
                atkScript.EndAttack();
                StartCoroutine(AttackParried(2f, parryKnockback));
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        atkScript.EndAttack();

        //follow-up with combo 2 if in range
        if (behaviorPhase != "start" && enemy.DistanceCheck(pb.gameObject.transform.position) < slashCombo2Range)
        {
            //check if player is in front of enemy
            float frontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(pb.gameObject.transform.position));

            if (frontCheck > 0)
            {
                StartCoroutine(SlashCombo2());
                yield break;
            }
        }

        yield return new WaitForSeconds(slashCombo1EndingLag); //ending lag

        actionInProgress = false;
    }

    private IEnumerator SlashCombo2()
    {
        AttackScript atkScript = slashCombo2Attack.GetComponent<AttackScript>();

        actionInProgress = true;
        readyForAttack = false;

        float atkTimer = 0;
        ResetWalkAnim();
        animator.SetTrigger("combo2");

        while (atkTimer < slashCombo2StartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(1.5f);

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        while (atkTimer < slashCombo2Duration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < slashCombo2MotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * slashCombo2Speed * Time.fixedDeltaTime);
            }

            if (attackParried)
            {
                atkScript.EndAttack();
                StartCoroutine(AttackParried(2f, parryKnockback));
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        atkScript.EndAttack();

        //follow-up with combo 3 if in range
        if (behaviorPhase != "start" && enemy.DistanceCheck(pb.gameObject.transform.position) < slashCombo3Range)
        {
            //check if player is in front of enemy
            float frontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(pb.gameObject.transform.position));

            if (frontCheck > 0)
            {
                StartCoroutine(SlashCombo3());
                yield break;
            }
        }

        yield return new WaitForSeconds(slashCombo2EndingLag); //ending lag

        actionInProgress = false;
    }

    private IEnumerator SlashCombo3()
    {
        AttackScript atkScript = slashCombo3Attack.GetComponent<AttackScript>();

        actionInProgress = true;
        readyForAttack = false;

        float atkTimer = 0;
        ResetWalkAnim();
        animator.SetTrigger("combo3");

        while (atkTimer < slashCombo3StartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(3f);

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        while (atkTimer < slashCombo3Duration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < 0.25f)
            {
                LookAtPlayer(1.5f);
            }
            else if (atkTimer > 0.25f && atkTimer < slashCombo3MotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * slashCombo3Speed * Time.fixedDeltaTime);
            }
            else if (atkTimer > slashCombo3MotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * slashCombo3Speed * 0.3f * (slashCombo3Duration/atkTimer) * Time.fixedDeltaTime);
            }

            if (attackParried)
            {
                atkScript.EndAttack();
                StartCoroutine(AttackParried(2f, parryKnockback));
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        atkScript.EndAttack();

        /* //follow-up with combo 4 if in range
        if (behaviorPhase != "start" && enemy.DistanceCheck(pb.gameObject.transform.position) < slashCombo3Range)
        {
            //check if player is in front of enemy
            float frontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(pb.gameObject.transform.position));

            if (frontCheck > 0)
            {
                StartCoroutine(SlashCombo3());
                yield break;
            }
        } */

        yield return new WaitForSeconds(slashCombo3EndingLag); //ending lag

        actionInProgress = false;
    }

    private IEnumerator HeavySlash()
    {
        AttackScript atkScript = heavySlashAttack.GetComponent<AttackScript>();

        actionInProgress = true;
        readyForAttack = false;

        float atkTimer = 0;
        ResetWalkAnim();
        animator.SetTrigger("chargedAttack");

        while (atkTimer < heavySlashStartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(1.4f);

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        while (atkTimer < heavySlashDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < 0.1f)
            {
                LookAtPlayer(1.2f);
            }
            else if (atkTimer > 0.1f && atkTimer < heavySlashMotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * heavySlashSpeed * Time.fixedDeltaTime);
                LookAtPlayer(1f);
            }
            else if (atkTimer > heavySlashMotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * heavySlashSpeed * 0.3f * (heavySlashDuration/atkTimer) * Time.fixedDeltaTime);
                //transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));
            }

            if (attackParried)
            {
                atkScript.EndAttack();
                StartCoroutine(AttackParried(2f, parryKnockback));
                yield break;
            }

            //atkScript.EndAttack();

            yield return new WaitForFixedUpdate();
        }

/*         //follow-up with combo 2 if in range
        if (behaviorPhase != "start" && enemy.DistanceCheck(pb.gameObject.transform.position) < slashCombo2Range)
        {
            //check if player is in front of enemy
            float frontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(pb.gameObject.transform.position));

            if (frontCheck > 0)
            {
                StartCoroutine(SlashCombo2());
                yield break;
            }
        } */

        yield return new WaitForSeconds(heavySlashEndingLag); //ending lag

        actionInProgress = false;
    }



    private IEnumerator MeteorShot()
    {
        //AttackScript atkScript = MeteorShotAttack.GetComponent<AttackScript>();

        actionInProgress = true;
        animator.SetTrigger("shotAttack");

        float atkTimer = 0;

        while (atkTimer < meteorShotStartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(0.33f);

            yield return new WaitForFixedUpdate();
        }

        //atkScript.StartAttack(); //enable hitbox
        Instantiate(meteorShot, transform.position + meteorShotOffset, transform.rotation);
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
        animator.SetTrigger("lightningAttack");

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

        animator.SetTrigger("endLightningAttack");

        //atkScript.EndAttack();

        yield return new WaitForSeconds(magicRingEndingLag); //ending lag

        actionInProgress = false;
    }

    private IEnumerator AttackParried(float stunTime, float knockback)
    {
        float timer = 0;
        Vector3 parryDir = transform.position - pb.gameObject.transform.position;
        parryDir.y = 0;

        animator.SetTrigger("gotParried");

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

    

    private void ResetWalkAnim()
    {
        animator.SetBool("walking", false);
        animator.SetFloat("motionFwd", 0);
        animator.SetFloat("motionSide", 0);
    }

    public void EndBattle()
    {
        this.enabled = false;
    }
}
