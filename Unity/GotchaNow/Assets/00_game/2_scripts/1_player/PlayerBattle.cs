using FMODUnity;
using System;
using System.Collections;
using System.Threading;

//using System.Numerics;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerIntermission))]
public class PlayerBattle : MonoBehaviour
{
    public static PlayerBattle Instance;
    public static event Action OnDeath;
    private InputSystem_Actions input = null;
    public BattleManager bm;
    public GameObject model;
    [HideInInspector] public Rigidbody rb = null;
    private Vector2 moveVector = Vector2.zero;
    private Vector2 camVector = Vector2.zero;
    private Vector2 stickPosition;
    public GameObject mainCamera;
    public GameObject freeCam;
    public GameObject lockOnCam;
    public Animator animator;
    private bool lockOnReady = true;

    [HideInInspector] public bool lockedOn = false;
    public GameObject lockOnTarget;
    private bool switchReadyR = true;
    private bool switchReadyL = true;

    //essential values
    public float maxHP = 100;
    public float HP = 100;
    public float moveSpeed = 0.5f;
    public float lockOnSpeedMult = 0.5f;
    public float dodgeSpeed = 5f;
    public float rotateSpeed = 10f;
    [HideInInspector] public bool death = false;
    public float deathCountdown = 0f;

    [HideInInspector] public bool hitStun = false;
    [HideInInspector] public float hitStunTimer = 0f;

    //public so enemies can check player state
    [HideInInspector] public bool actionInProgress = false;
    [HideInInspector] public bool dodgeActive = false;
    [HideInInspector] public bool guardActive = false;

    //hitbox objects
    public GameObject slash1;
    public GameObject slash2;
    public GameObject slash3;
    public GameObject heavySlash;
    public GameObject blockBox;

    private bool slashReady = true;
    private bool slash1Queued = false;
    private bool slash2Queued = false;
    private bool slash3Queued = false;
    private bool heavySlashReady = true;
    private bool confirmReady = true;

    public float slashCooldown = 1f;
    public float slash1Duration = 0.5f;
    public float slash2Duration = 0.5f;
    public float slash3Duration = 0.5f;

    public float slash2Window = 0.25f;
    public float slash3Window = 0.25f;

    public float heavySlashChargeTime = 0.66f;
    public float heavySlashCooldown = 1f;
    public float heavySlashDuration = 0.5f;

    public float slash1fMovement = 1f;
    public float slash1MotionTime = 0.3f;
    public float slash2fMovement = 1f;
    public float slash2MotionTime = 0.3f;
    public float slash3fMovement = 1f;
    public float slash3MotionTime = 0.3f;
    public float heavySlashfMovement = 1f;
    public float heavySlashMotionTime = 0.3f;
    public float attackRotationInfluence = 3f;

    private bool dodgeReady = true;
    private bool dodgeQueued = false;
    public float dodgeTime = 0.5f;
    public float dodgeMotionTime = 0.5f;
    public float dodgeInvulnerableTime = 0.2f;
    public float baseKnockback = 2f;
    public float hitStunTime = 0.8f;
    public bool invulnerable = false;
    [HideInInspector] public bool dodgeSuccessful = false;
    private bool blockReady = true;
    public BlockHitbox blockHitbox;
    [HideInInspector] public bool parrySuccessful = false;
    public bool buffActive = false;
    public GameObject buffVFX;
    public float buffTime;
    public float buffMult = 1.5f;
    public bool meteorExists = false;
    [Header("Sound FX")]
    public EventReference Slash1SFX;

    //colors
    /* public Material defaultMaterial;
    public Material dodgeMaterial;
    public Material hitstunMaterial; */

    /* private GameObject VFXBlur;
    private GameObject HitBloom; */

    private void Awake()
    {
        Instance = this;

        mainCamera = freeCam;

        input = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();

        //GetComponent<MeshRenderer>().material = defaultMaterial;

        //VFXBlur = GameObject.Find("VFXBlur");
        //HitBloom = GameObject.Find("VFXBloomWhite");
    }

    private void OnEnable()
    {
        //input system stuff
        input.Enable();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCanceled;

        //reset stats & bools
        HP = maxHP;
        
        actionInProgress = false;
        hitStun = false;

        buffActive = false;
        guardActive = false;
        invulnerable = false;

        slash1Queued = false;
        slash2Queued = false;
        slash3Queued = false;
        dodgeQueued = false;

        /* input.Player.CameraControl.performed += OnCameraPerformed;
        input.Player.CameraControl.canceled += OnCameraCanceled; */
    }

    private void OnDisable()
    {
        //input system stuff
        input.Disable();
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCanceled;

        /* input.Player.CameraControl.performed -= OnCameraPerformed; 
        input.Player.CameraControl.canceled -= OnCameraCanceled; */
    }

    private void Start()
    {
        NotificationManager.instance.BuffEnabled.AddListener(EnableBuff);
        NotificationManager.instance.BuffDisabled.AddListener(DisableBuff);
    }

    private void Update()
    {
        //check left analog
        stickPosition = new Vector2(moveVector.x, moveVector.y);

        //DEBUG
        if (input.Player.Down.IsPressed())
        {
            //bm.SetTimeScale(1f);
        }

        if (input.Player.Up.IsPressed())
        {
            //bm.SetTimeScale(0.01f);
        }

        //re-enable moves
        if (!slashReady)
        {
            if (!input.Player.Attack1.IsPressed())
            {
                slashReady = true;
            }
        }

        if (!heavySlashReady)
        {
            if (!input.Player.Attack2.IsPressed())
            {
                heavySlashReady = true;
            }
        }

        if (!dodgeReady)
        {
            if (!input.Player.Dodge.IsPressed())
            {
                dodgeReady = true;
            }
        }

        if (!lockOnReady)
        {
            if (!input.Player.LockOn.IsPressed())
            {
                lockOnReady = true;
            }
        }

        if (!switchReadyR)
        {
            if (!input.Player.LockOnR.IsPressed())
            {
                switchReadyR = true;
            }
        }

        if (!switchReadyL)
        {
            if (!input.Player.LockOnL.IsPressed())
            {
                switchReadyL = true;
            }
        }

        if (!blockReady)
        {
            if (!input.Player.Block.IsPressed())
            {
                blockReady = true;
            }
        }

        if (!confirmReady)
        {
            if (!input.Player.Confirm.IsPressed())
            {
                confirmReady = true;
            }
        }
    }

    private void FixedUpdate()
    {
        //player death
        if (HP <= 0)
        {
            if (death) return;
            death = true;

            //GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            OnDeath?.Invoke();
        }

        if (death == true)
        {
            if (deathCountdown == 0)
            {
                //DeathSFX audio;
            }

            deathCountdown += 1f;
            actionInProgress = true;
        }

        //movement
        moveCharacter(stickPosition);

        //lock on & target switching
        if (input.Player.LockOn.IsPressed() && lockOnReady)
        {
            if (lockedOn == false)
            {
                lockedOn = true;
                LockOn();

                if (!actionInProgress)
                {
                    StartCoroutine(RotateOnTargetSwitch());
                }
            }
            else if (lockedOn == true && !meteorExists)
            {
                lockedOn = false;
                LockOn();
            }

            lockOnReady = false;
        }

        if (input.Player.LockOnR.IsPressed() && switchReadyR && lockedOn && !meteorExists)
        {
            lockOnCam.GetComponent<LockOnCamera>().SwitchTargetR();

            if (!actionInProgress)
            {
                StartCoroutine(RotateOnTargetSwitch());
            }

            switchReadyR = false;
        }

        if (input.Player.LockOnL.IsPressed() && switchReadyL && lockedOn && !meteorExists)
        {
            lockOnCam.GetComponent<LockOnCamera>().SwitchTargetL();

            if (!actionInProgress)
            {
                StartCoroutine(RotateOnTargetSwitch());
            }

            switchReadyL = false;
        }

        //inputs for moveset
        if (!actionInProgress && !hitStun)
        {
            //attack
            if (input.Player.Attack1.IsPressed() && slashReady)
            {
                actionInProgress = true;
                slashReady = false;
                StartCoroutine(Slash1());
            }

            //heavy attack
            if (input.Player.Attack2.IsPressed() && heavySlashReady)
            {
                actionInProgress = true;
                heavySlashReady = false;
                StartCoroutine(HeavySlashCharge());
            }

            //block
            if (input.Player.Block.IsPressed() && blockReady)
            {
                actionInProgress = true;
                blockReady = false;
                StartCoroutine(StartBlock());
            }

            //dodge roll
            if (input.Player.Dodge.IsPressed() && dodgeReady)
            {
                actionInProgress = true;
                dodgeReady = false;
                StartCoroutine(Roll(stickPosition));

                //DodgeSFX audio;
            }
        }
        
    }

    //parry successful (TO DO!!)
    public void ParrySuccessful()
    {
        blockHitbox.ParryVFX();
    }

    //collision w enemy attack
    public void HitByAttack(float dmg, float atkKnockback, Vector3 attackDir)
    {
        if (invulnerable)
        {
            return;
        }
        
        if (buffActive) HP -= dmg * 0.5f;
        else HP -= dmg;
        
        hitStun = true;

        if (guardActive) //TO DO: IMPROVE (use EnemyAttackBox.attackBlocked)
        {
            hitStunTimer = hitStunTime/2;

            /* guardActive = false;
            blockReady = true;
            blockBox.GetComponent<BlockScript>().EndBlock(); */
        }
        else
        {
            hitStunTimer = hitStunTime;
            animator.SetTrigger("gotHit");
        }
        
        StartCoroutine(Knockback(atkKnockback, attackDir));

        //Debug.Log("Damage: " + dmg + " , HP: " + HP + "/" + maxHP);

        StartCoroutine(bm.EnemyAttackUI());
        StartCoroutine(bm.UpdatePlayerHP((HP + dmg) / maxHP, HP / maxHP));
    }

    private IEnumerator Knockback(float atkKnockback, Vector3 attackDir)
    {
        float timer = 0;
        float knockback = atkKnockback;
        StartCoroutine(Invulnerability(0.75f));
        //GetComponent<MeshRenderer>().material = hitstunMaterial;

        while (timer < hitStunTimer)
        {
            timer += Time.fixedDeltaTime;
            
            if (timer < hitStunTimer/2)
            {
                knockback = Mathf.Lerp(atkKnockback,0,timer/(hitStunTimer/2));
                rb.MovePosition(rb.position + attackDir.normalized * knockback * baseKnockback * Time.fixedDeltaTime);
            }

            yield return new WaitForFixedUpdate();
        }

        hitStun = false;
        //GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    private IEnumerator Invulnerability(float invTime)
    {
        float timer = 0;
        invulnerable = true;

        while (timer < invTime)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        invulnerable = false;
    }

    //movement
    private void moveCharacter(Vector2 direction) //movement
    {
        Vector3 camFwd = new Vector3(transform.position.x - mainCamera.transform.position.x, 0, transform.position.z - mainCamera.transform.position.z);
        float rotationY = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Vector3 moveDir = Quaternion.Euler(0, rotationY, 0) * camFwd.normalized;

        float tiltStrength = direction.magnitude; //analog movement speed
        animator.SetFloat("runIntensity", 0);

        if (!actionInProgress && !hitStun && direction.magnitude > 0.1f)
        {
            animator.SetFloat("runIntensity", tiltStrength);

            if (lockedOn) //slower movement when blocking
            {
                rb.MovePosition(rb.position + moveDir * tiltStrength * moveSpeed * lockOnSpeedMult * Time.fixedDeltaTime); //slow down when blocking
            }
            else
            {
                rb.MovePosition(rb.position + moveDir * tiltStrength * moveSpeed * Time.fixedDeltaTime);
            }

            if (lockedOn) //rotate towards lock on target
            {
                animator.SetFloat("walkDirSide", direction.x);
                animator.SetFloat("walkDirFwd", direction.y);

                Vector3 targetPos = lockOnTarget.GetComponent<LockOnTarget>().targetEnemy.transform.position;
                transform.LookAt(Vector3.Lerp(transform.position + transform.forward, new Vector3(targetPos.x, transform.position.y, targetPos.z), 0.15f));
            }
            else //rotate towards movement direction
            {
                transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.44f));
            }
        }
    }

    private IEnumerator StartBlock()
    {
        float timer = 0;
        float blockMinTime = 0.2f;
        
        guardActive = true;

        animator.SetTrigger("guard");
        animator.SetBool("guarding", true);

        while (timer < blockMinTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        blockBox.GetComponent<BlockScript>().StartBlock();
        
        while (timer >= blockMinTime)
        {
            timer += Time.deltaTime;

            if (!input.Player.Block.IsPressed()) //end blocking
            {
                EndBlock();
                yield break;
            }

            yield return null;
        }
    }
    
    public void EndBlock()
    {
        guardActive = false;
        animator.SetBool("guarding", false);
        blockBox.GetComponent<BlockScript>().EndBlock();

        actionInProgress = false;
    }

    //dodge roll
    private IEnumerator Roll(Vector3 direction)
    {
        float timer = 0;
        Vector3 camFwd = new Vector3(transform.position.x - mainCamera.transform.position.x, 0, transform.position.z - mainCamera.transform.position.z);
        float rotationY = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Vector3 moveDir = Quaternion.Euler(0, rotationY, 0) * camFwd.normalized;

        invulnerable = true;

        animator.SetTrigger("dodge");
        //model.GetComponent<Renderer>().material.color = Color.yellow; //debug

        while (timer < dodgeTime)
        {
            timer += Time.deltaTime;

            if (timer < dodgeMotionTime)
            {
                rb.MovePosition(rb.position + moveDir * dodgeSpeed * Time.fixedDeltaTime);
                transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));
            }
            else
            {
                rb.MovePosition(rb.position + moveDir * dodgeSpeed * 0.3f * (dodgeTime / timer) * Time.fixedDeltaTime);
                transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));
            }

            if (timer > dodgeInvulnerableTime)
            {
                invulnerable = false;
                model.GetComponent<Renderer>().material.color = Color.white; //debug
            }

            if (input.Player.Attack1.IsPressed() && slashReady)
            {
                slash1Queued = true;
                slashReady = false;
            }

            yield return new WaitForFixedUpdate();
        }

        if (slash1Queued)
        {
            slash1Queued = false;
            StartCoroutine(Slash1());
        }
        else
        {
            actionInProgress = false;
        }
    }

    //attack combo
    private IEnumerator Slash1()
    {
        AttackScript atkScript = slash1.GetComponent<AttackScript>();

        animator.SetTrigger("attack1");
        RuntimeManager.PlayOneShot(Slash1SFX, transform.position);
        
        atkScript.StartAttack(); //enable hitbox
        float atkTimer = 0;

        while (atkTimer < slash1Duration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer > 0.1f && atkTimer < slash1MotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * slash1fMovement * Time.fixedDeltaTime);
                //transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));
            }

            if (atkTimer > slash1Duration - slash2Window)
            {
                if (input.Player.Attack1.IsPressed() && slashReady && !dodgeQueued) //queue combo attack
                {
                    slash2Queued = true;
                    slashReady = false;
                    atkTimer *= 1.5f;
                }
                else if (input.Player.Dodge.IsPressed() && dodgeReady && !slash2Queued) //queue dodge cancel
                {
                    dodgeQueued = true;
                    dodgeReady = false;
                    atkTimer *= 1.5f;
                }
            }

            yield return new WaitForFixedUpdate();
        }

        //atkScript.EndAttack(); //disable hitbox

        if (slash2Queued) //go to combo atk
        {
            StartCoroutine(Slash2());

            slash2Queued = false;
            yield break;
        }
        else if (dodgeQueued) //cancel into dodge
        {
            StartCoroutine(Roll(stickPosition));

            dodgeQueued = false;
            yield break;
        }

        yield return new WaitForSeconds(slashCooldown); //ending lag

        actionInProgress = false;
    }

    private IEnumerator Slash2()
    {
        AttackScript atkScript = slash2.GetComponent<AttackScript>();

        animator.SetTrigger("attack2");

        atkScript.StartAttack(); //enable hitbox
        float atkTimer = 0;

        while (atkTimer < slash2Duration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer < slash2MotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * slash2fMovement * Time.fixedDeltaTime);
                //transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));
            }
            
            if (atkTimer > slash2Duration - slash3Window)
            {
                if (input.Player.Attack1.IsPressed() && slashReady && !dodgeQueued) //queue combo attack
                {
                    slash3Queued = true;
                    slashReady = false;
                    atkTimer *= 1.2f;
                }
                else if (input.Player.Dodge.IsPressed() && dodgeReady && !slash2Queued) //queue dodge cancel
                {
                    dodgeQueued = true;
                    dodgeReady = false;
                    atkTimer *= 1.5f;
                }
            }

            yield return new WaitForFixedUpdate();
        }

        //atkScript.EndAttack(); //disable hitbox

        if (slash3Queued) //go to combo attack
        {
            StartCoroutine(Slash3());
            
            slash3Queued = false;
            yield break;
        }
        else if (dodgeQueued) //cancel into dodge
        {
            StartCoroutine(Roll(stickPosition));

            dodgeQueued = false;
            yield break;
        }

        yield return new WaitForSeconds(slashCooldown); //ending lag

        actionInProgress = false;
    }
    
    private IEnumerator Slash3()
    {
        AttackScript atkScript = slash3.GetComponent<AttackScript>();

        animator.SetTrigger("attack3");

        atkScript.StartAttack(); //enable hitbox
        float atkTimer = 0;

        while (atkTimer < slash3Duration)
        {
            atkTimer += Time.fixedDeltaTime;

            if (atkTimer > 0.25f && atkTimer < slash3MotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * slash3fMovement * Time.fixedDeltaTime);
                //transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));
            }
            else if (atkTimer > slash3MotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * slash3fMovement * 0.3f * (slash3Duration/atkTimer) * Time.fixedDeltaTime);
                //transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));
            }
            
            yield return new WaitForFixedUpdate();
        }

        //atkScript.EndAttack(); //disable hitbox

        yield return new WaitForSeconds(slashCooldown); //ending lag

        actionInProgress = false;
    }

    //heavy attack
    private IEnumerator HeavySlashCharge()
    {
        float chgTimer = 0;
        animator.SetTrigger("chargeHeavy");
        animator.SetBool("charging", true);

        while (chgTimer < heavySlashChargeTime)
        {
            chgTimer += Time.deltaTime;

            if (chgTimer > heavySlashChargeTime / 2)
            {
                //
            }

            if (!input.Player.Attack2.IsPressed())
            {
                model.GetComponent<Renderer>().material.color = Color.white; //debug
                if (chgTimer > heavySlashChargeTime / 2) //lower power attack before full charge
                {
                    StartCoroutine(HeavySlash(0.5f + 0.25f * (chgTimer / heavySlashChargeTime)));
                    animator.SetBool("charging", false);

                    yield break;
                }
                else //cancel charge
                {
                    actionInProgress = false;
                    animator.SetBool("charging", false);
                    yield break;
                }
            }

            yield return null;
        }

        StartCoroutine(HeavySlash(1)); //full power charge attack
        animator.SetBool("charging", false);
    }

    private IEnumerator HeavySlash(float chgAmount)
    {
        AttackScript atkScript = heavySlash.GetComponent<AttackScript>();

        atkScript.StartAttack(); //enable hitbox

        animator.SetTrigger("heavyAttack");

        /* AttackBox atkBox = heavySlash.GetComponentInChildren<AttackBox>();
        float baseDmg = atkBox.damage;
        atkBox.damage *= chgAmount; //calculate damage */

        float atkTimer = 0;

        while (atkTimer < heavySlashDuration)
        {
            if (atkTimer > 0.1f && atkTimer < heavySlashMotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * heavySlashfMovement * Time.fixedDeltaTime);
                //transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));
            }
            else if (atkTimer > heavySlashMotionTime)
            {
                rb.MovePosition(rb.position + transform.forward * heavySlashfMovement * 0.3f * (slash3Duration/atkTimer) * Time.fixedDeltaTime);
                //transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));
            }

            atkTimer += Time.deltaTime;

            // Debug.Log("test");
            yield return new WaitForFixedUpdate();
        }

        /* atkBox.damage = baseDmg; //reset damage of hitbox */
        //atkScript.EndAttack();

        yield return new WaitForSeconds(heavySlashCooldown); //ending lag

        actionInProgress = false;
    }

    //switch camera modes
    public void LockOn()
    {
        if (mainCamera == freeCam) //switch from free cam to lockon
        {
            animator.SetBool("lockedOn", true);
            
            lockOnCam.SetActive(true);

            lockOnCam.GetComponent<LockOnCamera>().isActive = true;
            lockOnCam.GetComponent<LockOnCamera>().SetTarget(); //find target enemy

            mainCamera = lockOnCam;

            //TO DO: IMPROVE CAM SWITCHING
            mainCamera.transform.position = freeCam.transform.position;
            mainCamera.transform.rotation = freeCam.transform.rotation;

            freeCam.SetActive(false);
        }

        else if (mainCamera == lockOnCam) //switch from lockon to free cam
        {
            animator.SetBool("lockedOn", false);
            
            freeCam.SetActive(true);
            lockOnCam.GetComponent<LockOnCamera>().isActive = false;

            mainCamera = freeCam;

            //TO DO: IMPROVE CAM SWITCHING
            mainCamera.GetComponent<CinemachineOrbitalFollow>().ForceCameraPosition(lockOnCam.transform.position, lockOnCam.transform.rotation);

            lockOnCam.SetActive(false);
        }
    }

    //rotate character towards new target when locked on
    private IEnumerator RotateOnTargetSwitch()
    {
        float rotationTime = 0f;
        while (rotationTime < 0.1f)
        {
            rotationTime += Time.deltaTime;
            Vector3 targetPos = lockOnTarget.GetComponent<LockOnTarget>().targetEnemy.transform.position;
            transform.LookAt(Vector3.Lerp(transform.position + transform.forward, new Vector3(targetPos.x, transform.position.y, targetPos.z), 0.33f));

            yield return new WaitForFixedUpdate();
        }
    }
    
    //REQUESTS
    public void Heal(float hpAmount)
    {
        RuntimeManager.PlayOneShot(UiSfxPlayer.instance.heal, transform.position); //play sfx
        
        if (HP + hpAmount >= maxHP)
        {
            StartCoroutine(BattleManager.instance.UpdatePlayerHP(HP/maxHP, 1));
            HP = maxHP;
        }
        else
        {
            StartCoroutine(BattleManager.instance.UpdatePlayerHP(HP/maxHP, (HP+hpAmount)/maxHP));
            HP += hpAmount;
        }
    }

    public void EnableBuff()
    {   
        buffActive = true;
        StartCoroutine(BuffTimer());
    }

    private IEnumerator BuffTimer()
    {
        float timer = 0;

        yield return new WaitForSeconds(0.5f);

        buffVFX.SetActive(true);
        RuntimeManager.PlayOneShot(UiSfxPlayer.instance.buffActivate, transform.position); //play sfx

        while (timer < buffTime)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        DisableBuff();
    }

    public void DisableBuff()
    {
        buffVFX.SetActive(false);
        RuntimeManager.PlayOneShot(UiSfxPlayer.instance.buffDeactivate, transform.position); //play sfx
        
        buffActive = false;
    }

    public void EndBattle()
    {
        StopAllCoroutines();
        EndBlock();
        DisableBuff();

        animator.SetFloat("runIntensity", 0);
        animator.SetBool("charging", false);
        
        if (lockedOn)
        {
            lockedOn = false;
            LockOn();

            animator.SetBool("lockedOn", false);
            animator.SetFloat("walkDirSide", 0);
            animator.SetFloat("walkDirFwd", 0);
        }
        
        GetComponent<PlayerIntermission>().enabled = true;
        enabled = false;
    }

    //input system stuff for left analog movement
    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }
    private void OnMovementCanceled(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;
    }

    //input system stuff for right analog movement (maybe unneeded)
    /* private void OnCameraPerformed(InputAction.CallbackContext value)
    {
        camVector = value.ReadValue<Vector2>();
    }
    private void OnCameraCanceled(InputAction.CallbackContext value)
    {
        camVector = Vector2.zero;
    } */
}