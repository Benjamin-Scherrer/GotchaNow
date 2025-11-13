using System.Collections;
using System.Threading;
using Unity.Properties;
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
    private string behaviorState = null;
    private string attackState = null;
    [Header("Basic Attributes")]
    public float walkSpeed = 3;
    public float strafeSpeed = 2;
    public float turnSpeed = 0.3f;
    
    [Header("Shoulder Bash Attack")]
    public GameObject ShoulderBashAttack;
    public float shoulderBashRange;
    public float shoulderBashStartupTime;
    public float shoulderBashDuration;
    public float shoulderBashMotionTime;
    public float shoulderBashSpeed;
    public float shoulderBashEndingLag;
    [Header("Claw Swipe Attack")]
    public GameObject ClawSwipeAttack;
    public float clawSwipeRange;
    public float clawSwipeStartupTime;
    public float clawSwipeDuration;
    public float clawSwipeMotionTime;
    public float clawSwipeSpeed;
    public float clawSwipeEndingLag;
    [Header("Hammer Jump Attack")]
    public GameObject HammerJumpAttack;
    public AnimationCurve hammerJumpCurve;
    public float hammerJumpRange;
    public float hammerJumpStartupTime;
    public float hammerJumpAirTime;
    public float hammerJumpYPos;
    public float hammerJumpHeight;
    public float hammerJumpSpeed;
    public float hammerJumpDuration;
    public float hammerJumpEndingLag;
    [Header("Hammer Spin Attack")]
    public GameObject HammerSpinAttack;
    public float hammerSpinRange;
    public float hammerSpinStartupTime;
    public float hammerSpinDuration;
    public float hammerSpinMotionTime;
    public float hammerSpinSpeed;
    public float hammerSpinEndingLag;
    [Header("Hammer Combo 1 Attack")]
    public GameObject HammerCombo1Attack;
    public float hammerCombo1Range;
    public float hammerCombo1StartupTime;
    public float hammerCombo1Duration;
    public float hammerCombo1MotionTime;
    public float hammerCombo1Speed;
    public float hammerCombo1EndingLag;
    [Header("Hammer Combo 2 Attack")]
    public GameObject HammerCombo2Attack;
    public float hammerCombo2Range;
    public float hammerCombo2StartupTime;
    public float hammerCombo2Duration;
    public float hammerCombo2MotionTime;
    public float hammerCombo2Speed;
    public float hammerCombo2EndingLag;
    [Header("Parry")]
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

        /* if (distance <= shoulderBashRange)
        {
            StartCoroutine(ShoulderBash());
        } */
        /* if (distance <= clawSwipeRange)
        {
            StartCoroutine(ClawSwipe());
        } */
        /* if (distance <= hammerJumpRange)
        {
            StartCoroutine(HammerJump());
        } */
        /* if (distance <= hammerSpinRange)
        {
            StartCoroutine(HammerSpin());
        } */
        if (distance <= hammerCombo1Range)
        {
            StartCoroutine(HammerCombo1());
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

    //SHOULDER BASH
    private IEnumerator ShoulderBash()
    {
        AttackScript atkScript = ShoulderBashAttack.GetComponent<AttackScript>();

        float atkTimer = 0;

        while (atkTimer < shoulderBashStartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(1.25f);

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        while (atkTimer < shoulderBashDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < shoulderBashMotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * shoulderBashSpeed * Time.fixedDeltaTime);
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

        yield return new WaitForSeconds(shoulderBashEndingLag); //ending lag

        actionInProgress = false;
    }

    //CLAW SWIPE
    private IEnumerator ClawSwipe()
    {
        AttackScript atkScript = ClawSwipeAttack.GetComponent<AttackScript>();

        float atkTimer = 0;

        while (atkTimer < clawSwipeStartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(0.25f);

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        while (atkTimer < clawSwipeDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < clawSwipeMotionTime)
            {   
                rb.MovePosition(rb.position + transform.forward * clawSwipeSpeed * Time.fixedDeltaTime);
                transform.LookAt(Vector3.Lerp(rb.position + transform.forward, rb.position + transform.right, 0.02f));
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

        yield return new WaitForSeconds(clawSwipeEndingLag); //ending lag

        actionInProgress = false;
    }

    //HAMMER JUMP
    private IEnumerator HammerJump()
    {
        AttackScript atkScript = HammerJumpAttack.GetComponent<AttackScript>();

        float atkTimer = 0;

        while (atkTimer < hammerJumpStartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(1.25f);

            yield return new WaitForFixedUpdate();
        }

        Vector3 startingSpot = transform.position;
        Vector3 landingSpot = pb.transform.position;
        landingSpot.y = transform.position.y;
        Vector3 jumpVector = landingSpot - transform.position;

        atkTimer = 0;

        while (atkTimer < hammerJumpAirTime)
        {
            atkTimer += Time.fixedDeltaTime;

            hammerJumpYPos = startingSpot.y + hammerJumpHeight * hammerJumpCurve.Evaluate(atkTimer / hammerJumpAirTime);
            rb.MovePosition(new Vector3(startingSpot.x + jumpVector.x * (atkTimer / hammerJumpAirTime), hammerJumpYPos, startingSpot.z + jumpVector.z * (atkTimer / hammerJumpAirTime)));

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        while (atkTimer < hammerJumpDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (attackParried)
            {
                atkScript.EndAttack();
                StartCoroutine(AttackParried(2f, parryKnockback));
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        atkScript.EndAttack();

        yield return new WaitForSeconds(hammerJumpEndingLag); //ending lag

        actionInProgress = false;
    }

    //HAMMER SPIN
    private IEnumerator HammerSpin()
    {
        AttackScript atkScript = HammerSpinAttack.GetComponent<AttackScript>();

        float atkTimer = 0;

        while (atkTimer < hammerSpinStartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(0.33f);

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        Vector3 moveDir = rb.position + transform.forward;

        while (atkTimer < hammerSpinDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < hammerSpinMotionTime)
            {
                Vector3 pVector = pb.gameObject.transform.position - rb.position;
                pVector.y = 0;

                rb.MovePosition(rb.position + pVector.normalized * hammerSpinSpeed * Time.fixedDeltaTime);
                //moveDir = Vector3.Lerp(moveDir, rb.position + pVector.normalized, 0.5f);

                transform.LookAt(Vector3.Lerp(rb.position + transform.forward, rb.position + transform.right, 0.1f));
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

        yield return new WaitForSeconds(hammerSpinEndingLag); //ending lag

        actionInProgress = false;
    }

    private IEnumerator HammerCombo1()
    {
        AttackScript atkScript = HammerCombo1Attack.GetComponent<AttackScript>();

        float atkTimer = 0;

        while (atkTimer < hammerCombo1StartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(0.5f);

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        while (atkTimer < hammerCombo1Duration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < hammerCombo1MotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * hammerCombo1Speed * Time.fixedDeltaTime);
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

        //follow-up with combo 2
        distance = enemy.DistanceCheck(pb.gameObject.transform.position);

        if (distance < hammerCombo2Range)
        {
            //check if player is in front of enemy
            float frontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(pb.gameObject.transform.position));

            if (frontCheck > 0)
            {
                StartCoroutine(HammerCombo2());
                yield break;
            }
        }

        yield return new WaitForSeconds(hammerCombo1EndingLag); //ending lag

        actionInProgress = false;
    }
    
    private IEnumerator HammerCombo2()
    {
        AttackScript atkScript = HammerCombo2Attack.GetComponent<AttackScript>();

        float atkTimer = 0;

        while (atkTimer < hammerCombo2StartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(0.5f);

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        while (atkTimer < hammerCombo2Duration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < hammerCombo2MotionTime)
            {   
                rb.MovePosition(rb.position + transform.forward * hammerCombo2Speed * Time.fixedDeltaTime);
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

        yield return new WaitForSeconds(hammerCombo2EndingLag); //ending lag

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
        /* AttackScript atkScript = attack1.GetComponent<AttackScript>();
        atkScript.EndAttack(); */

        this.enabled = false;
    }
}
