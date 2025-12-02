using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    private bool avoidActive = false;
    private List<string> availableAttacks = new List<string>();
    private List<string> attackChoice = new List<string>();

    [Header("Basic Attributes")]
    public float walkSpeed = 3;
    public float strafeSpeed = 2;
    public float turnSpeed = 0.3f;
    public float maxAvoidRange = 25f;
    [Header("Meteor Shot Attack")]
    public GameObject meteorShot;
    public float meteorShotMinDistance;
    public float meteorShotRange;
    public float meteorShotStartupTime;
    public float meteorShotDuration;
    public float meteorShotMotionTime;
    public float meteorShotSpeed;
    public float meteorShotEndingLag;
    [Header("Horizontal Lightning Shot Attack")]
    public GameObject horizontalLightningShot;
    public float horizontalLightningMinDistance;
    public float horizontalLightningRange;
    public float horizontalLightningStartupTime;
    public float horizontalLightningDuration;
    public float horizontalLightningMotionTime;
    public float horizontalLightningSpeed;
    public float horizontalLightningEndingLag;
    [Header("Magic Ring Attack")]
    public GameObject magicRing;
    public float magicRingMinDistance;
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
    [Header("Backdash")]
    public float backdashMaxOffCenter;
    public float backdashStartupTime;
    public float backdashDuration;
    public float backdashSpeed;
    public float backdashEndingLag;
    [Header("Special Attack")]
    public bool specialAttackReady = false;
    public float specialAttackStartupTime;
    public float specialAttackDuration;
    public float specialAttackMotionTime;
    public float specialAttackSpeed;
    public float specialAttackEndingLag;
    [Header("Parry etc")]
    public bool attackParried = false;
    public float parryKnockback = 10f;
    private bool gotHit = false;
    public float hitTime = 0.1f;
    public float baseKnockback = 1f;
    public Transform arenaCenter;
    public TextMeshProUGUI debugText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemy = GetComponent<Enemy>();
    }

    void OnEnable()
    {
        actionInProgress = false;

        availableAttacks.Clear();
        
        behaviorPhase = "start";
        availableAttacks.Add("slashCombo");
        availableAttacks.Add("heavySlash");
        availableAttacks.Add("meteorShot");
    }
    
    void Start()
    {
        pb = PlayerBattle.Instance;
    }

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
        if (avoidActive) debugText.text += "\navoid active";
        debugText.text += "\nHP: " + enemy.HP + "/" + enemy.maxHP; 
        
        //set moves on new phase
        if (behaviorPhase == "start" && (enemy.HP/enemy.maxHP < 0.85))
        {            
            behaviorPhase = "phase1";

            //specialAttackReady = true;

            availableAttacks.Clear();
            availableAttacks.Add("slashCombo");
            availableAttacks.Add("heavySlash");
            availableAttacks.Add("magicRing");
            availableAttacks.Add("meteorShot");
        }

        if (behaviorPhase == "phase1" && (enemy.HP/enemy.maxHP < 0.4))
        {
            behaviorPhase = "phase2";

            specialAttackReady = true;

            availableAttacks.Clear();
            availableAttacks.Add("slashCombo");
            availableAttacks.Add("heavySlash");
            availableAttacks.Add("horizontalLightning");
            availableAttacks.Add("meteorShot");
            availableAttacks.Add("magicRing");
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
        if (specialAttackReady)
        {
            StartCoroutine(SpecialAttack());
            specialAttackReady = false;
            return;
        }
        
        distance = enemy.DistanceCheck(pb.gameObject.transform.position);
        Debug.Log("distance to player: " + distance);

        int rndAction;

        if (behaviorPhase == "start")
        {
            rndAction = Random.Range(0,3);
            if (rndAction == 0) StartCoroutine(ApproachAction(3));
            if (rndAction == 1) StartCoroutine(StrafeAction(3));
            if (rndAction == 2) StartCoroutine(AvoidAction(4));
        }
        if (behaviorPhase == "phase1")
        {
            rndAction = Random.Range(0,5);
            if (rndAction == 0) StartCoroutine(ApproachAction(4));
            if (rndAction == 1) StartCoroutine(StrafeAction(3));
            if (rndAction == 2) StartCoroutine(AvoidAction(5));
            if (rndAction == 3) StartCoroutine(MeteorShot());
            if (rndAction == 4) StartCoroutine(HorizontalLightningShot());
        }
        if (behaviorPhase == "phase2")
        {
            rndAction = Random.Range(0,7);
            if (rndAction == 0) StartCoroutine(ApproachAction(3));
            if (rndAction == 1) StartCoroutine(StrafeAction(3));
            if (rndAction == 2) StartCoroutine(AvoidAction(5));
            if (rndAction == 3) StartCoroutine(MeteorShot());
            if (rndAction == 4) StartCoroutine(HorizontalLightningShot());
            if (rndAction == 5) StartCoroutine(MagicRing());
            if (rndAction == 6) StartCoroutine(Backdash());
        }

        actionInProgress = true;
        
        /* 
        //StartCoroutine(HeavySlash());
        //StartCoroutine(SlashCombo1());
        //StartCoroutine(MeteorShot());
        //StartCoroutine(MagicRing());
        //StartCoroutine(HorizontalLightningShot());
        //LookAtPlayer(5f);
        //StartCoroutine(Backdash()); 
        // */
    }

    //attack choice behavior
    private void PickAttack()
    {
        distance = enemy.DistanceCheck(pb.gameObject.transform.position);

        //find attacks in range & add to attackChoice List
        for (int i = 0; i < availableAttacks.Count; i++)
        {
            if (availableAttacks[i] == "slashCombo" && distance <= slashCombo1Range)
            {
                attackChoice.Add("slashCombo");
            }

            if (availableAttacks[i] == "heavySlash" && distance <= heavySlashRange)
            {
                attackChoice.Add("heavySlash");
            }

            if (availableAttacks[i] == "horizontalLightning" && distance >= horizontalLightningMinDistance && distance <= horizontalLightningRange)
            {
                attackChoice.Add("horizontalLightning");
            }

            if (availableAttacks[i] == "meteorShot" && distance >= meteorShotMinDistance && distance <= meteorShotRange)
            {
                attackChoice.Add("meteorShot");
            }

            if (availableAttacks[i] == "magicRing" && distance >= magicRingMinDistance && distance <= magicRingRange)
            {
                attackChoice.Add("magicRing");
            }
        }

        //pick attack when in range
        if (attackChoice.Count > 0)
        {
            int rndAttack = Random.Range(0, attackChoice.Count);

            if (strafeActive) strafeActive = false;
            else if (approachActive) approachActive = false;
            else if (avoidActive) avoidActive = false;

            StopAllCoroutines();
                
            if (attackChoice[rndAttack] == "slashCombo")
            {
                StartCoroutine(SlashCombo1());
            }
            else if (attackChoice[rndAttack] == "heavySlash")
            {
                StartCoroutine(HeavySlash());
            }
            else if (attackChoice[rndAttack] == "horizontalLightning")
            {
                StartCoroutine(HorizontalLightningShot());
            }
            else if (attackChoice[rndAttack] == "meteorShot")
            {
                StartCoroutine(MeteorShot());
            }
            else if (attackChoice[rndAttack] == "magicRing")
            {
                StartCoroutine(MagicRing());
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
        animator.SetBool("walking", true);

        if (behaviorPhase == "start") readyForAttack = false;
        else if (behaviorPhase == "phase1" || behaviorPhase == "phase2") readyForAttack = true;

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

                if (behaviorPhase == "start" && (timer > actionTime/3))
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
                int rndmMove = UnityEngine.Random.Range(0,4);
                if (rndmMove == 0 || rndmMove == 1) StartCoroutine(ApproachAction(4));
                else if (rndmMove == 1) StartCoroutine(StrafeAction(6));
                else if (rndmMove == 3) StartCoroutine(MeteorShot());
            }
            else if (behaviorPhase == "phase1")
            {
                int rndmMove = UnityEngine.Random.Range(0,5);
                if (rndmMove == 0) StartCoroutine(ApproachAction(4));
                else if (rndmMove == 1) StartCoroutine(StrafeAction(5));
                else if (rndmMove == 2) StartCoroutine(AvoidAction(5));
                else if (rndmMove == 3) StartCoroutine(MeteorShot());
                else if (rndmMove == 4) StartCoroutine(HorizontalLightningShot());
            }
            else if (behaviorPhase == "phase2")
            {
                int rndmMove = UnityEngine.Random.Range(0,7);
                if (rndmMove == 0) StartCoroutine(ApproachAction(4));
                else if (rndmMove == 1) StartCoroutine(StrafeAction(5));
                else if (rndmMove == 2) StartCoroutine(AvoidAction(5));
                else if (rndmMove == 3 || rndmMove == 4) StartCoroutine(MeteorShot());
                else if (rndmMove == 5 || rndmMove == 6) StartCoroutine(HorizontalLightningShot());
            }

            //actionInProgress = false;
            approachActive = false;
        }
    }

    private IEnumerator StrafeAction(float actionTime)
    {
        float timer = 0;

        strafeActive = true;
        animator.SetBool("walking", true);

        if (behaviorPhase == "start" || behaviorPhase == "phase1") readyForAttack = false;
        else if ( behaviorPhase == "phase2") readyForAttack = true;

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
                if (readyForAttack == false && behaviorPhase == "phase1" && (timer > actionTime/3))
                {
                    readyForAttack = true;
                }

                LookAtPlayer(0.75f);
                WalkTowardsPlayer(0.33f);
                StrafeAroundPlayer(strafeDir, 1);

                yield return new WaitForFixedUpdate();
            }

            if (behaviorPhase == "start")
            {
                int rndmMove = UnityEngine.Random.Range(0,3);
                if (rndmMove == 0) StartCoroutine(ApproachAction(5));
                else if (rndmMove == 1) StartCoroutine(AvoidAction(6));
                else if (rndmMove == 2) StartCoroutine(MeteorShot());
            }
            else if (behaviorPhase == "phase1")
            {
                if (BackDashCheck())
                {
                    StartCoroutine(Backdash());
                }
                else
                {
                    int rndmMove = UnityEngine.Random.Range(0,5);
                    if (rndmMove == 0) StartCoroutine(ApproachAction(5));
                    else if (rndmMove == 1) StartCoroutine(AvoidAction(6));
                    else if (rndmMove == 2) StartCoroutine(MeteorShot());
                    else if (rndmMove == 3) StartCoroutine(HorizontalLightningShot());
                    else if (rndmMove == 4) StartCoroutine(MagicRing());
                }
                
            }
            else if (behaviorPhase == "phase2")
            {
                if (BackDashCheck())
                {
                    StartCoroutine(Backdash());
                }
                else
                {
                    int rndmMove = UnityEngine.Random.Range(0,8);
                    if (rndmMove == 0) StartCoroutine(ApproachAction(5));
                    else if (rndmMove == 1) StartCoroutine(AvoidAction(5));
                    else if (rndmMove == 2 || rndmMove == 3) StartCoroutine(MeteorShot());
                    else if (rndmMove == 4 || rndmMove == 5) StartCoroutine(HorizontalLightningShot());
                    else if (rndmMove == 6) StartCoroutine(MagicRing());
                    else if (rndmMove == 7) StartCoroutine(Backdash());
                }
            }

            //actionInProgress = false;
            strafeActive = false;
        }        
    }

    private IEnumerator AvoidAction(float actionTime)
    {
        if (enemy.DistanceCheck(pb.gameObject.transform.position) > maxAvoidRange)
        {
            int rndmMove = UnityEngine.Random.Range(0,5);
            if (rndmMove == 0 || rndmMove == 1 || rndmMove == 2) StartCoroutine(ApproachAction(6));
            else if (rndmMove == 3) StartCoroutine(MagicRing());
            else if (rndmMove == 4) StartCoroutine(MeteorShot());

            yield break;
        }
        
        float timer = 0;

        avoidActive = true;
        animator.SetBool("walking", true);

        if (behaviorPhase == "start" || behaviorPhase == "phase1" ) readyForAttack = false;
        else if (behaviorPhase == "phase2") readyForAttack = true;

        //WIP: set strafe direction
        int rndm = UnityEngine.Random.Range(0, 2);
        string strafeDir;
        if (rndm == 0) strafeDir = "l";
        else strafeDir = "r";

        while (avoidActive)
        {
            while (timer < actionTime)
            {
                timer += Time.fixedDeltaTime;

                if (readyForAttack == false && behaviorPhase == "start" && (timer > actionTime/2))
                {
                    readyForAttack = true;
                }
                if (readyForAttack == false && behaviorPhase == "phase1" && (timer > actionTime/3))
                {
                    readyForAttack = true;
                }

                LookAtPlayer(0.75f);
                AvoidPlayer(1f);
                StrafeAroundPlayer(strafeDir, 0.33f);

                if (behaviorPhase != "start" && (timer > actionTime/2))
                {
                    BackDashCheck();
                }

                

                yield return new WaitForFixedUpdate();
            }

            if (behaviorPhase == "start")
            {
                int rndmMove = UnityEngine.Random.Range(0,2);
                if (rndmMove == 0) StartCoroutine(StrafeAction(3));
                else if (rndmMove == 1) StartCoroutine(MeteorShot());
            }
            else if (behaviorPhase == "phase1")
            {
                if (BackDashCheck())
                {
                    StartCoroutine(Backdash());
                }
                else
                {
                    int rndmMove = UnityEngine.Random.Range(0,4);
                    if (rndmMove == 0) StartCoroutine(StrafeAction(3));
                    else if (rndmMove == 1) StartCoroutine(MeteorShot());
                    else if (rndmMove == 2) StartCoroutine(HorizontalLightningShot());
                    else if (rndmMove == 3) StartCoroutine(MagicRing());
                }
                
            }
            else if (behaviorPhase == "phase2")
            {
                if (BackDashCheck())
                {
                    StartCoroutine(Backdash());
                }
                else
                {
                    int rndmMove = UnityEngine.Random.Range(0,8);
                    if (rndmMove == 0) StartCoroutine(StrafeAction(3));
                    if (rndmMove == 1) StartCoroutine(ApproachAction(4));
                    else if (rndmMove == 2 || rndmMove == 3) StartCoroutine(MeteorShot());
                    else if (rndmMove == 4 || rndmMove == 5) StartCoroutine(HorizontalLightningShot());
                    else if (rndmMove == 6) StartCoroutine(MagicRing());
                    else if (rndmMove == 7) StartCoroutine(Backdash());
                }
            }

            //actionInProgress = false;
            avoidActive = false;
        }        
    }

    private bool BackDashCheck()
    {
        float centerFrontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(arenaCenter.transform.position));
        float playerFrontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(PlayerBattle.Instance.transform.position));

        //Debug.Log(centerFrontCheck + " " + playerFrontCheck);

        if (centerFrontCheck > 0 || playerFrontCheck < 0)
        {
            return false;
        }
        else return true;
    }
    
    private IEnumerator Backdash()
    {   
        float timer = 0;
        Vector3 newPosition = arenaCenter.transform.position - (PlayerBattle.Instance.transform.position-arenaCenter.transform.position).normalized * backdashMaxOffCenter;
        newPosition.y = 2.5f;

        actionInProgress = true;
        ResetWalkAnim();
        animator.SetTrigger("backdash");

        while (timer < backdashStartupTime)
        {
            timer += Time.fixedDeltaTime;
            LookAtPlayer(1.25f);

            yield return new WaitForFixedUpdate();
        }

        while (timer < backdashDuration)
        {
            timer += Time.fixedDeltaTime;

            Vector3 direction = newPosition - transform.position;

            LookAtPlayer(1.5f);
            rb.MovePosition(rb.position + direction * backdashSpeed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }

        if (enemy.DistanceCheck(pb.gameObject.transform.position) < horizontalLightningRange)
        {
            StartCoroutine(HorizontalLightningShot());
            yield break;
        }
        
        yield return new WaitForSeconds(backdashEndingLag);
        {
            int rndmMove = UnityEngine.Random.Range(0,3);
            if (rndmMove == 0 || rndmMove == 1) StartCoroutine(MeteorShot());
            else if (rndmMove == 2) StartCoroutine(MagicRing());
        }

        //actionInProgress = false;
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
                StartCoroutine(AttackParried(1f, parryKnockback));
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        atkScript.EndAttack();

        //follow-up with combo 2 if in range
        if (enemy.DistanceCheck(pb.gameObject.transform.position) < slashCombo2Range)
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
            LookAtPlayer(1.25f);

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
                StartCoroutine(AttackParried(1f, parryKnockback));
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
            LookAtPlayer(1.75f);

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        while (atkTimer < slashCombo3Duration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < 0.25f)
            {
                float frontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(PlayerBattle.Instance.transform.position));
                if (frontCheck > 0) LookAtPlayer(1.25f);
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
                StartCoroutine(AttackParried(1.25f, parryKnockback));
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        atkScript.EndAttack();

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
            LookAtPlayer(1.25f);

            yield return new WaitForFixedUpdate();
        }

        atkScript.StartAttack(); //enable hitbox
        atkTimer = 0;

        while (atkTimer < heavySlashDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < 0.1f)
            {
                float frontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(PlayerBattle.Instance.transform.position));
                if (frontCheck > 0) LookAtPlayer(0.7f);
            }
            else if (atkTimer > 0.1f && atkTimer < heavySlashMotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * heavySlashSpeed * Time.fixedDeltaTime);
                
                float frontCheck = Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(PlayerBattle.Instance.transform.position));
                if (frontCheck > 0) LookAtPlayer(0.4f);
            }
            else if (atkTimer > heavySlashMotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * heavySlashSpeed * 0.3f * (heavySlashDuration/atkTimer) * Time.fixedDeltaTime);
                //transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));
            }

            if (attackParried)
            {
                atkScript.EndAttack();
                StartCoroutine(AttackParried(1.25f, parryKnockback));
                yield break;
            }

            //atkScript.EndAttack();

            yield return new WaitForFixedUpdate();
        }

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
            LookAtPlayer(0.2f);

            yield return new WaitForFixedUpdate();
        }

        //atkScript.StartAttack(); //enable hitbox
        Instantiate(meteorShot, transform.position + transform.forward + 1.5f * transform.up, transform.rotation);
        atkTimer = 0;

        while (atkTimer < meteorShotDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < meteorShotMotionTime)
            {   
                LookAtPlayer(0.33f);
            }

            yield return new WaitForFixedUpdate();
        }

        if (behaviorPhase != "start")
        {
            int rndRepeat = Random.Range(0,2);

            if (rndRepeat == 0)
            {
                Instantiate(meteorShot, transform.position + transform.forward + 1.5f * transform.up, transform.rotation);
                atkTimer = 0;

                while (atkTimer < meteorShotDuration)
                {
                    atkTimer += Time.fixedDeltaTime;

                    if (atkTimer < meteorShotMotionTime)
                    {   
                        LookAtPlayer(0.33f);
                    }

                yield return new WaitForFixedUpdate();
                }
            }
        }

        animator.SetTrigger("endShotAttack");

        yield return new WaitForSeconds(meteorShotEndingLag); //ending lag

        actionInProgress = false;
    }

    private IEnumerator HorizontalLightningShot()
    {
        //HorizontalLightningAttack atkScript = MeteorShotAttack.GetComponent<HorizontalLightningAttack>();

        actionInProgress = true;
        animator.SetTrigger("shotAttack");

        float atkTimer = 0;

        while (atkTimer < horizontalLightningStartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            LookAtPlayer(0.33f);

            yield return new WaitForFixedUpdate();
        }

        //atkScript.StartAttack(); //enable hitbox
        GameObject shot = Instantiate(horizontalLightningShot, transform.position + transform.forward + 1.4f * transform.up, transform.rotation, transform);    
        shot.GetComponent<HorizontalLightningAttack>().lifetime = horizontalLightningDuration;
        atkTimer = 0;

        while (atkTimer < horizontalLightningDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < horizontalLightningMotionTime)
            {   
                LookAtPlayer(0.25f);
            }

            yield return new WaitForFixedUpdate();
        }

        animator.SetTrigger("endShotAttack");

        yield return new WaitForSeconds(horizontalLightningEndingLag); //ending lag

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
        GameObject trackingRing = Instantiate(magicRing, PlayerBattle.Instance.transform.position, transform.rotation);
        trackingRing.GetComponent<MagicRingAttack>().trackingTarget = PlayerBattle.Instance.transform;

        atkTimer = 0;

        while (atkTimer < magicRingDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < magicRingMotionTime)
            {   
                LookAtPlayer(0.25f);
            }

            yield return new WaitForFixedUpdate();
        }

        animator.SetTrigger("endLightningAttack");

        //atkScript.EndAttack();

        yield return new WaitForSeconds(magicRingEndingLag); //ending lag

        actionInProgress = false;
    }

    private IEnumerator SpecialAttack()
    {
        actionInProgress = true;
        
        animator.SetTrigger("backdash");

        float atkTimer = 0;

        while (atkTimer < specialAttackStartupTime/2)
        {
            atkTimer += Time.fixedDeltaTime;
            
            Vector3 direction = arenaCenter.position - transform.position;

            LookAtPlayer(1f);
            rb.MovePosition(rb.position + transform.up + direction * backdashSpeed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }

        animator.SetTrigger("lightningAttack");

        GameObject omenRing = Instantiate(magicRing, transform.position, transform.rotation);
        omenRing.GetComponent<MagicRingAttack>().attackDelay = specialAttackStartupTime/2;
        omenRing.GetComponent<MagicRingAttack>().lifeTime = specialAttackStartupTime;

        while (atkTimer < specialAttackStartupTime)
        {
            atkTimer += Time.fixedDeltaTime;
            
            Vector3 direction = arenaCenter.position - transform.position;

            LookAtPlayer(1f);

            yield return new WaitForFixedUpdate();
        }

        animator.SetTrigger("shotAttack");

        StartCoroutine(VfxManager.instance.SpecialAttackFlash(5, specialAttackDuration));

        //atkScript.StartAttack(); //enable hitbox
        GameObject trackingRing = Instantiate(magicRing, PlayerBattle.Instance.transform.position, transform.rotation);
        trackingRing.GetComponent<MagicRingAttack>().trackingTarget = PlayerBattle.Instance.transform;

        GameObject spawnRing1 = Instantiate(magicRing, transform.position + transform.right * 8, transform.rotation);
        spawnRing1.GetComponent<MagicRingAttack>().spawner = true;

        GameObject spawnRing2 = Instantiate(magicRing, transform.position - transform.right * 8, transform.rotation);
        spawnRing2.GetComponent<MagicRingAttack>().spawner = true;

        Instantiate(meteorShot, transform.position + transform.up + transform.forward * 4, transform.rotation);

        Instantiate(meteorShot, transform.position + transform.up + transform.right * 4, transform.rotation);

        Instantiate(meteorShot, transform.position + transform.up - transform.right * 4, transform.rotation);

        Instantiate(magicRing, transform.position + transform.forward * 8, transform.rotation);

        Instantiate(magicRing, transform.position - transform.forward * 8, transform.rotation);

        Instantiate(magicRing, transform.position + transform.forward * 6 + transform.right * 6, transform.rotation);

        Instantiate(magicRing, transform.position + transform.forward * 6 - transform.right * 6, transform.rotation);

        Instantiate(magicRing, transform.position - transform.forward * 6 + transform.right * 6, transform.rotation);

        Instantiate(magicRing, transform.position - transform.forward * 6 - transform.right * 6, transform.rotation);

        var rotation = Quaternion.LookRotation(transform.forward);

        GameObject shot1 = Instantiate(horizontalLightningShot, transform.position + transform.forward + 1.4f * transform.up, transform.rotation, transform);    
        shot1.GetComponent<HorizontalLightningAttack>().lifetime = specialAttackDuration;

        rotation *= Quaternion.Euler(0, 90, 0); // this adds a 90 degrees Y rotation
        GameObject shot2 = Instantiate(horizontalLightningShot, transform.position + transform.right + 1.4f * transform.up, rotation, transform);    
        shot2.GetComponent<HorizontalLightningAttack>().lifetime = specialAttackDuration;
        
        rotation *= Quaternion.Euler(0, 90, 0); // this adds a 90 degrees Y rotation
        GameObject shot3 = Instantiate(horizontalLightningShot, transform.position - transform.forward + 1.4f * transform.up, rotation, transform);    
        shot3.GetComponent<HorizontalLightningAttack>().lifetime = specialAttackDuration;
        
        rotation *= Quaternion.Euler(0, 90, 0); // this adds a 90 degrees Y rotation
        GameObject shot4 = Instantiate(horizontalLightningShot, transform.position - transform.right + 1.4f * transform.up, rotation, transform);    
        shot4.GetComponent<HorizontalLightningAttack>().lifetime = specialAttackDuration;

        atkTimer = 0;

        while (atkTimer < specialAttackDuration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < specialAttackMotionTime)
            {   
                //LookAtPlayer(0.2f);
            }

            yield return new WaitForFixedUpdate();
        }

        animator.SetTrigger("endShotAttack");

        //atkScript.EndAttack();

        yield return new WaitForSeconds(specialAttackEndingLag); //ending lag

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

    public IEnumerator GotHit(float atkKnockback)
    {
        gotHit = true;
        Debug.Log("got hit");

        float timer = 0f;
        float knockback = atkKnockback;

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

        gotHit = false;
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
