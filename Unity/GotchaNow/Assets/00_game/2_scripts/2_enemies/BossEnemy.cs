using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossEnemy : MonoBehaviour
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
    private bool gotHit = false;
    public float hitTime = 0.1f;
    public float baseKnockback = 1f;
    [Header("Visual")]
    public Renderer eyeRenderer;
    public Renderer gemRenderer;
    private Color eyeColor;
    private Color gemColor;
    public Color defaultEyeColor;
    public Color defaultGemColor;
    public GameObject gemExplosion;
    public TextMeshProUGUI debugText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemy = GetComponent<Enemy>();
    }

    void OnEnable()
    {
        pb = PlayerBattle.Instance;

        StartBattle();
    }

    public void StartBattle()
    {   
        eyeColor = defaultEyeColor;
        gemColor = defaultGemColor;

        eyeRenderer.material.SetColor("_Diamond_Color", eyeColor);
        gemRenderer.material.SetColor("_Diamond_Color", gemColor);

        actionInProgress = false;

        availableAttacks.Clear();
        
        behaviorPhase = "start";
        availableAttacks.Add("hammerCombo");
        availableAttacks.Add("clawSwipe");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        debugText.text = "behavior phase: " + behaviorPhase + "\navailable Attacks:\n";
        for (int i = 0; i < availableAttacks.Count; i++)
        {
            debugText.text += availableAttacks[i] + "\n";
        }
        if (readyForAttack) debugText.text += "\nready for attack";
        if (strafeActive) debugText.text += "\nstrafe active";
        if (approachActive) debugText.text += "\napproach active";
        debugText.text += "\nHP: " + enemy.HP + "/" + enemy.maxHP; 
        
        //set moves on new phase
        if (behaviorPhase == "start" && (enemy.HP/enemy.maxHP < 0.8))
        {
            eyeColor = Color.orange;
            gemColor = Color.orange;
            eyeRenderer.material.SetColor("_Diamond_Color", eyeColor);
            gemRenderer.material.SetColor("_Diamond_Color", gemColor);
            
            behaviorPhase = "phase1";

            availableAttacks.Clear();
            availableAttacks.Add("hammerCombo");
            availableAttacks.Add("clawSwipe");
            availableAttacks.Add("shoulderBash");
        }

        if (behaviorPhase == "phase1" && (enemy.HP/enemy.maxHP < 0.4))
        {
            eyeColor = Color.darkRed;
            gemColor = Color.darkRed;
            eyeRenderer.material.SetColor("_Diamond_Color", eyeColor);
            gemRenderer.material.SetColor("_Diamond_Color", gemColor);
            
            behaviorPhase = "phase2";

            availableAttacks.Clear();
            availableAttacks.Add("hammerCombo");
            availableAttacks.Add("clawSwipe");
            availableAttacks.Add("shoulderBash");
            availableAttacks.Add("hammerSpin");
        }
        
        if (readyForAttack)
        {
            PickAttack();
        }

        if (!actionInProgress)
        {
            ChooseAction();
        }
    }

    private void ChooseAction()
    {
        distance = enemy.DistanceCheck(pb.gameObject.transform.position);
        Debug.Log("distance to player: " + distance);

        int rndAction;

        if (behaviorPhase == "start")
        {
            rndAction = Random.Range(0,2);
            if (rndAction == 0) StartCoroutine(ApproachAction(5));
            if (rndAction == 1) StartCoroutine(StrafeAction(5));
        }
        if (behaviorPhase == "phase1")
        {
            rndAction = Random.Range(0,2);
            if (rndAction == 0) StartCoroutine(ApproachAction(5));
            if (rndAction == 1) StartCoroutine(StrafeAction(5));
        }
        if (behaviorPhase == "phase2")
        {
            rndAction = Random.Range(0,3);
            if (rndAction == 0) StartCoroutine(ApproachAction(5));
            if (rndAction == 1) StartCoroutine(StrafeAction(5));
            if (rndAction == 2) StartCoroutine(HammerJump());
        }

        actionInProgress = true;
    }

    //attack choice behavior
    private void PickAttack()
    {
        distance = enemy.DistanceCheck(pb.gameObject.transform.position);

        //find attacks in range & add to attackChoice List
        for (int i = 0; i < availableAttacks.Count; i++)
        {
            if (availableAttacks[i] == "clawSwipe" && distance <= clawSwipeRange)
            {
                attackChoice.Add("clawSwipe");
            }

            if (availableAttacks[i] == "hammerCombo" && distance <= clawSwipeRange)
            {
                attackChoice.Add("hammerCombo");
            }

            if (availableAttacks[i] == "hammerSpin" && distance <= clawSwipeRange)
            {
                attackChoice.Add("hammerSpin");
            }

            if (availableAttacks[i] == "hammerJump" && distance <= clawSwipeRange)
            {
                attackChoice.Add("hammerJump");
            }

            if (availableAttacks[i] == "shoulderBash" && distance <= clawSwipeRange)
            {
                attackChoice.Add("shoulderBash");
            }
        }

        //pick attack when in range
        if (attackChoice.Count > 0)
        {
            int rndAttack = Random.Range(0, attackChoice.Count);

            if (strafeActive) strafeActive = false;
            else if (approachActive) approachActive = false;

            StopAllCoroutines();
                
            if (attackChoice[rndAttack] == "clawSwipe")
            {
                StartCoroutine(ClawSwipe());
            }
            else if (attackChoice[rndAttack] == "hammerCombo")
            {
                StartCoroutine(HammerCombo1());
            }
            else if (attackChoice[rndAttack] == "hammerSpin")
            {
                StartCoroutine(HammerSpin());
            }
            else if (attackChoice[rndAttack] == "hammerJump")
            {
                StartCoroutine(HammerJump());
            }
            else if (attackChoice[rndAttack] == "shoulderBash")
            {
                StartCoroutine(ShoulderBash());
            }
            
            attackChoice.Clear();
            readyForAttack = false;
        }
    }

    //MOVEMENT FUNCTIONS
    private void LookAtPlayer(float speedMult)
    {
        Vector3 pVector = pb.gameObject.transform.position - transform.position;
        pVector.y = 0;

        transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + pVector.normalized, turnSpeed * speedMult));
    }

    private void WalkTowardsPlayer(float speedMult)
    {
        Vector3 pVector = pb.gameObject.transform.position - rb.position;
        pVector.y = 0;

        animator.SetFloat("motionFwd", 1);
        rb.MovePosition(rb.position + pVector.normalized * walkSpeed * speedMult * Time.fixedDeltaTime);
    }
    
    private void AvoidPlayer(float speedMult)
    {
        Vector3 pVector = pb.gameObject.transform.position - rb.position;
        pVector.y = 0;

        animator.SetFloat("motionFwd", -1);
        rb.MovePosition(rb.position - pVector.normalized * walkSpeed * speedMult * Time.fixedDeltaTime);
    }

    private void StrafeAroundPlayer(string dir, float speedMult)
    {
        if (dir == "r")
        {
            animator.SetFloat("motionSide", 1);
            rb.MovePosition(rb.position + transform.right * strafeSpeed * speedMult * Time.fixedDeltaTime);
        }

        if (dir == "l")
        {
            animator.SetFloat("motionSide", -1);
            rb.MovePosition(rb.position - transform.right * strafeSpeed * speedMult * Time.fixedDeltaTime);
        }
    }

    //MOVEMENT COROUTINES
    private IEnumerator ApproachAction(float actionTime)
    {
        float timer = 0;

        approachActive = true;
        animator.SetBool("Walking", true);

        if (behaviorPhase == "start" || behaviorPhase == "phase1") readyForAttack = false;
        else if (behaviorPhase == "phase2") readyForAttack = true;

        //WIP: set strafe direction
        int rndm = Random.Range(0, 2);
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

            if (behaviorPhase == "start")
            {
                StartCoroutine(ShoulderBash());
                
                /* int rndmMove = Random.Range(0,3);
                if (rndmMove == 0) StartCoroutine(ApproachAction(4));
                else if (rndmMove == 1) StartCoroutine(StrafeAction(6));
                else if (rndmMove == 2) StartCoroutine(ShoulderBash()); */
            }
            else if (behaviorPhase == "phase1")
            {
                int rndmMove = Random.Range(0,4);
                if (rndmMove == 0) StartCoroutine(ApproachAction(4));
                else if (rndmMove == 1) StartCoroutine(StrafeAction(6));
                else if (rndmMove == 2) StartCoroutine(ShoulderBash());
                else if (rndmMove == 3) StartCoroutine(HammerJump());
            }
            else if (behaviorPhase == "phase2")
            {
                int rndmMove = Random.Range(0,6);
                if (rndmMove == 0) StartCoroutine(ApproachAction(3));
                else if (rndmMove == 1) StartCoroutine(StrafeAction(5));
                else if (rndmMove == 2 || rndmMove == 3) StartCoroutine(ShoulderBash());
                else if (rndmMove == 4 || rndmMove == 5) StartCoroutine(HammerJump());
            }

            //actionInProgress = false;
            approachActive = false;
        }
    }

    private IEnumerator StrafeAction(float actionTime)
    {
        float timer = 0;

        strafeActive = true;
        animator.SetBool("Walking", true);

        if (behaviorPhase == "start" || behaviorPhase == "phase1") readyForAttack = false;
        else if (behaviorPhase == "phase2") readyForAttack = true;

        //WIP: set strafe direction
        int rndm = Random.Range(0, 2);
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

            if (behaviorPhase == "start")
            {
                int rndmMove = Random.Range(0,2);
                if (rndmMove == 0) StartCoroutine(ApproachAction(6));
                else if (rndmMove == 1) StartCoroutine(ShoulderBash());
            }
            else if (behaviorPhase == "phase1")
            {
                int rndmMove = Random.Range(0,3);
                if (rndmMove == 0) StartCoroutine(ApproachAction(5));
                else if (rndmMove == 1) StartCoroutine(ShoulderBash());
                else if (rndmMove == 2) StartCoroutine(HammerJump());
            }
            else if (behaviorPhase == "phase2")
            {
                int rndmMove = Random.Range(0,5);
                if (rndmMove == 0) StartCoroutine(ApproachAction(5));
                else if (rndmMove == 1 || rndmMove == 2) StartCoroutine(ShoulderBash());
                else if (rndmMove == 3 || rndmMove == 4) StartCoroutine(HammerJump());
            }

            //actionInProgress = false;
            strafeActive = false;
        }        
    }

    //ATTACKS
    private IEnumerator ShoulderBash()
    {
        AttackScript atkScript = ShoulderBashAttack.GetComponent<AttackScript>();

        actionInProgress = true;
        readyForAttack = false;

        float atkTimer = 0;
        ResetWalkAnim();
        animator.SetTrigger("ShoulderAttack");

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

        Debug.Log("End Shoulder Bash");
        animator.SetTrigger("ShoulderEnd");

        //follow-up with claw swipe
        if (behaviorPhase == "phase2" && enemy.DistanceCheck(pb.gameObject.transform.position) < clawSwipeRange)
        {
            //check if player is in front of enemy
            float frontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(pb.gameObject.transform.position));

            if (frontCheck > 0)
            {
                StartCoroutine(ClawSwipe());
                yield break;
            }
        }

        yield return new WaitForSeconds(shoulderBashEndingLag); //ending lag

        actionInProgress = false;
    }

    private IEnumerator ClawSwipe()
    {
        AttackScript atkScript = ClawSwipeAttack.GetComponent<AttackScript>();

        actionInProgress = true;
        readyForAttack = false;

        float atkTimer = 0;
        ResetWalkAnim();
        animator.SetTrigger("ClawAttack");

        while (atkTimer < clawSwipeStartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(0.33f);

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

    private IEnumerator HammerJump()
    {
        AttackScript atkScript = HammerJumpAttack.GetComponent<AttackScript>();

        actionInProgress = true;
        readyForAttack = false;

        float atkTimer = 0;
        ResetWalkAnim();
        animator.SetTrigger("JumpAttack");

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

    private IEnumerator HammerSpin()
    {
        AttackScript atkScript = HammerSpinAttack.GetComponent<AttackScript>();

        actionInProgress = true;
        readyForAttack = false;

        float atkTimer = 0;
        ResetWalkAnim();
        animator.SetTrigger("SpinAttack");

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

                //transform.LookAt(Vector3.Lerp(rb.position + transform.forward, rb.position + transform.right, 0.05f));
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
        animator.SetTrigger("SpinEnd");

        yield return new WaitForSeconds(hammerSpinEndingLag); //ending lag

        actionInProgress = false;
    }
    
    private IEnumerator HammerCombo1()
    {
        AttackScript atkScript = HammerCombo1Attack.GetComponent<AttackScript>();

        actionInProgress = true;
        readyForAttack = false;

        float atkTimer = 0;
        ResetWalkAnim();
        animator.SetTrigger("Combo1");

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

        //follow-up with combo 2 if in range
        if (behaviorPhase != "start" && enemy.DistanceCheck(pb.gameObject.transform.position) < hammerCombo2Range)
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

        actionInProgress = true;
        readyForAttack = false;

        float atkTimer = 0;
        animator.SetTrigger("Combo2");

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
    private IEnumerator AttackParried(float stunTime, float parryKnockback)
    {
        actionInProgress = true;
        
        float timer = 0;
        float knockback = parryKnockback;

        animator.SetTrigger("GotParried");

        Vector3 parryDir = transform.position - pb.gameObject.transform.position;
        parryDir.y = 0;

        while (timer < stunTime)
        {
            timer += Time.deltaTime;

            if (knockback > 0)
            {
                rb.MovePosition(transform.position + parryDir.normalized * knockback * Time.fixedDeltaTime);
                knockback = Mathf.Lerp(parryKnockback,0,3*timer/stunTime);
            }

            yield return new WaitForFixedUpdate();
        }

        attackParried = false;
        actionInProgress = false;
    }

    public IEnumerator GotHit(float atkKnockback)
    {
        gotHit = true;
        Debug.Log("got hit");

        float timer = 0f;
        float knockback = atkKnockback;

        eyeRenderer.material.SetColor("_Diamond_Color", Color.ghostWhite);
        gemRenderer.material.SetColor("_Diamond_Color", Color.ghostWhite);

        Vector3 attackDir = transform.position - pb.gameObject.transform.position;
        attackDir.y = 0;

        while (timer < hitTime)
        {
            timer += Time.fixedDeltaTime;
            
            if (knockback > 0)
            {
                rb.MovePosition(rb.position + attackDir.normalized * knockback * baseKnockback * Time.fixedDeltaTime);
                knockback = Mathf.Lerp(atkKnockback,0,4*timer/hitTime);
            }

            yield return new WaitForFixedUpdate();
        }

        eyeRenderer.material.SetColor("_Diamond_Color", eyeColor);
        gemRenderer.material.SetColor("_Diamond_Color", gemColor);

        gotHit = false;
    }

    private void ResetWalkAnim()
    {
        animator.SetBool("Walking", false);
        animator.SetFloat("motionFwd", 0);
        animator.SetFloat("motionSide", 0);
    }
    
    public void EndBattle()
    {
        /* AttackScript atkScript = attack1.GetComponent<AttackScript>();
        atkScript.EndAttack(); */
        StopAllCoroutines();
        
        ResetWalkAnim();
        animator.SetTrigger("GotParried"); //placeholder for death anim

        GameObject explosion = Instantiate(gemExplosion, transform.position, Quaternion.Euler(-90,0,0));
        explosion.transform.localScale *= 2;
        
        eyeColor = defaultEyeColor;
        gemColor = defaultGemColor;
        eyeRenderer.material.SetColor("_Diamond_Color", eyeColor);
        gemRenderer.material.SetColor("_Diamond_Color", gemColor);

        GetComponent<EnemyIntermission>().enabled = true;
        this.enabled = false;
    }
}
