using System;
using System.Collections;
//using System.Numerics;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBattle : MonoBehaviour
{
    public static PlayerBattle Instance;
    public static event Action OnDeath;
    private InputSystem_Actions input = null;
    public BattleManager bm;
    [HideInInspector] public Rigidbody rb = null;
    private Vector2 moveVector = Vector2.zero;
    private Vector2 camVector = Vector2.zero;
    private Vector3 stickPosition;
    private float rotationY;
    public GameObject mainCamera;
    public GameObject freeCam;
    public GameObject lockOnCam;
    private bool lockOnReady = true;

    [HideInInspector] public bool lockedOn = false;
    public GameObject lockOnTarget;
    private bool switchReadyR = true;
    private bool switchReadyL = true;

    //essential values
    public float maxHP = 100;
    public float HP = 100;
    public float moveSpeed = 0.5f;
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
    public float slash2fMovement = 1f;
    public float slash3fMovement = 1f;
    public float attackRotationInfluence = 3f;

    private bool dodgeReady = true;
    private bool dodgeQueued = false;
    public float dodgeCooldown = 1f;
    private float dodgeTimer = 0f;
    private Vector3 dodgeDir;
    [HideInInspector] public bool dodgeSuccessful = false;

    private bool blockReady = true;
    [HideInInspector] public bool parrySuccessful = false;

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

    private void Update()
    {
        //check left analog
        stickPosition = new Vector3(moveVector.x, 0, moveVector.y);

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

        if (!dodgeReady && !dodgeActive)
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
            else if (lockedOn == true)
            {
                lockedOn = false;
                LockOn();
            }

            lockOnReady = false;
        }

        if (input.Player.LockOnR.IsPressed() && switchReadyR && lockedOn)
        {
            lockOnCam.GetComponent<LockOnCamera>().SwitchTargetR();

            if (!actionInProgress)
            {
                StartCoroutine(RotateOnTargetSwitch());
            }

            switchReadyR = false;
        }

        if (input.Player.LockOnL.IsPressed() && switchReadyL && lockedOn)
        {
            lockOnCam.GetComponent<LockOnCamera>().SwitchTargetL();

            if (!actionInProgress)
            {
                StartCoroutine(RotateOnTargetSwitch());
            }

            switchReadyL = false;
        }

        //when hit by enemy attack
        /* if (hitStun)
        {
            hitStunTimer -= Time.deltaTime;
            //GetComponent<MeshRenderer>().material = hitstunMaterial;

            if (hitStunTimer <= 0)
            {
                //GetComponent<MeshRenderer>().material = defaultMaterial;
                hitStun = false;
            }
        } */

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
                guardActive = true;
                blockReady = false;
                blockBox.GetComponent<BlockScript>().StartBlock();
            }

            //dodge
            if (input.Player.Dodge.IsPressed() && dodgeReady)
            {
                actionInProgress = true;

                dodgeDir = stickPosition;
                dodgeActive = true;
                dodgeReady = false;
                dodgeTimer = dodgeCooldown;

                //DodgeSFX audio;
            }
        }

        //block active
        if (guardActive)
        {
            //

            if (!input.Player.Block.IsPressed()) //end blocking
            {
                guardActive = false;
                blockBox.GetComponent<BlockScript>().EndBlock();
            }
        }

        //dodge active
        if (dodgeActive)
        {
            if (dodgeSuccessful)
            {
                //PerfectDodgeSFX audio;

                dodgeSuccessful = false;
            }

            DodgeRoll(dodgeDir);
            dodgeTimer -= 1 * Time.fixedDeltaTime;
            //GetComponent<MeshRenderer>().material = dodgeMaterial;

            if (dodgeTimer <= 0)
            {
                dodgeActive = false;
                rb.linearVelocity = Vector3.zero;
                //GetComponent<MeshRenderer>().material = defaultMaterial;

                if (slash1Queued)
                {
                    slash1Queued = false;
                    slashReady = false;
                    StartCoroutine(Slash1());
                }
                else
                {
                    actionInProgress = false;
                }
            }
        }
    }

    public void ParrySuccessful()
    {
        Debug.Log("parry successful!");
    }

    public void HitByAttack(float dmg, float atkKnockback, Vector3 attackDir)
    {
        HP -= dmg;
        hitStun = true;

        if (guardActive)
        {
            hitStunTimer = 0.3f;
            guardActive = false;
            blockReady = true;
            blockBox.GetComponent<BlockScript>().EndBlock();
        }
        else
        {
            hitStunTimer = 1f;
        }        

        StartCoroutine(Knockback(atkKnockback, attackDir));

        //Debug.Log("Damage: " + dmg + " , HP: " + HP + "/" + maxHP);

        StartCoroutine(bm.EnemyAttackUI());
        StartCoroutine(bm.UpdatePlayerHP((HP + dmg) / maxHP, HP / maxHP));
    }
    
    private IEnumerator Knockback(float atkKnockback, Vector3 attackDir)
    {
        float knockback = atkKnockback;
        //GetComponent<MeshRenderer>().material = hitstunMaterial;

        while (hitStunTimer > 0)
        {
            if (atkKnockback > 0)
            {
                rb.MovePosition(rb.position + attackDir.normalized * atkKnockback * Time.fixedDeltaTime);
                atkKnockback -= Time.deltaTime * 4;
            }

            hitStunTimer -= Time.deltaTime;

            yield return null;
        }

        hitStun = false;
        //GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    private void moveCharacter(Vector3 direction) //movement
    {
        Vector3 camFwd = new Vector3(transform.position.x - mainCamera.transform.position.x, 0, transform.position.z - mainCamera.transform.position.z);
        rotationY = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg;
        Vector3 moveDir = Quaternion.Euler(0, rotationY, 0) * camFwd.normalized;

        float tiltStrength = direction.magnitude; //analog movement speed

        if (!actionInProgress && !hitStun && direction.magnitude > 0.1f)
        {
            if (guardActive) //slower movement when blocking
            {
                rb.MovePosition(rb.position + moveDir * tiltStrength * moveSpeed * 0.33f * Time.fixedDeltaTime); //slow down when blocking
            }
            else
            {
                rb.MovePosition(rb.position + moveDir * tiltStrength * moveSpeed * Time.fixedDeltaTime);
            }

            if (lockedOn) //rotate towards lock on target
            {
                Vector3 targetPos = lockOnTarget.GetComponent<LockOnTarget>().targetEnemy.transform.position;
                transform.LookAt(Vector3.Lerp(transform.position + transform.forward, new Vector3(targetPos.x, transform.position.y, targetPos.z), 0.15f));
            }
            else //rotate towards movement direction
            {
                transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.25f));
            }
        }
    }

    private void DodgeRoll(Vector3 direction) //dodge movement
    {
        Vector3 camFwd = new Vector3(transform.position.x - mainCamera.transform.position.x, 0, transform.position.z - mainCamera.transform.position.z);
        rotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Vector3 moveDir = Quaternion.Euler(0, rotationY, 0) * camFwd.normalized;

        rb.linearVelocity = moveDir * dodgeSpeed;
        transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));

        if (input.Player.Attack1.IsPressed() && slashReady)
        {
            slash1Queued = true;
            slashReady = false;
            dodgeTimer *= 0.5f;
        }
    }

    //attack combo
    private IEnumerator Slash1()
    {
        AttackScript atkScript = slash1.GetComponent<AttackScript>();

        atkScript.StartAttack(); //enable hitbox
        float atkTimer = 0;

        while (atkTimer < slash1Duration)
        {
            atkTimer += Time.deltaTime;

            if (atkTimer > slash1Duration - slash2Window)
            {
                if (input.Player.Attack1.IsPressed() && slashReady && !dodgeQueued) //queue combo attack
                {
                    slash2Queued = true;
                    slashReady = false;
                }
                else if (input.Player.Dodge.IsPressed() && dodgeReady && !slash2Queued) //queue dodge cancel
                {
                    dodgeQueued = true;
                    dodgeReady = false;
                    atkTimer *= 1.5f;
                }
            }

            yield return null;
        }

        atkScript.EndAttack(); //disable hitbox

        if (slash2Queued) //go to combo atk
        {
            StartCoroutine(Slash2());
            slash2Queued = false;
            yield break;
        }
        else if (dodgeQueued) //cancel into dodge
        {
            dodgeDir = stickPosition;
            dodgeActive = true;
            dodgeTimer = dodgeCooldown;

            dodgeQueued = false;
            yield break;
        }

        yield return new WaitForSeconds(slashCooldown); //ending lag

        actionInProgress = false;
    }

    private IEnumerator Slash2()
    {
        AttackScript atkScript = slash2.GetComponent<AttackScript>();

        atkScript.StartAttack(); //enable hitbox
        float atkTimer = 0;

        while (atkTimer < slash2Duration)
        {
            atkTimer += Time.deltaTime;

            if (atkTimer > slash2Duration - slash3Window)
            {
                if (input.Player.Attack1.IsPressed() && slashReady && !dodgeQueued) //queue combo attack
                {
                    slash3Queued = true;
                    slashReady = false;
                }
                else if (input.Player.Dodge.IsPressed() && dodgeReady && !slash2Queued) //queue dodge cancel
                {
                    dodgeQueued = true;
                    dodgeReady = false;
                    atkTimer *= 1.5f;
                }
            }

            yield return null;
        }

        atkScript.EndAttack(); //disable hitbox

        if (slash3Queued) //go to combo attack
        {
            StartCoroutine(Slash3());
            slash3Queued = false;
            yield break;
        }
        else if (dodgeQueued) //cancel into dodge
        {
            dodgeDir = stickPosition;
            dodgeActive = true;
            dodgeTimer = dodgeCooldown;

            dodgeQueued = false;
            yield break;
        }

        yield return new WaitForSeconds(slashCooldown); //ending lag

        actionInProgress = false;
    }
    
    private IEnumerator Slash3()
    {
        AttackScript atkScript = slash3.GetComponent<AttackScript>();

        atkScript.StartAttack(); //enable hitbox
        float atkTimer = 0;

        while (atkTimer < slash3Duration)
        {
            atkTimer += Time.deltaTime;
            yield return null;
        }

        atkScript.EndAttack(); //disable hitbox

        yield return new WaitForSeconds(slashCooldown); //ending lag

        actionInProgress = false;
    }

    //heavy attack
    private IEnumerator HeavySlashCharge()
    {
        float chgTimer = 0;

        while (chgTimer < heavySlashChargeTime)
        {
            chgTimer += Time.deltaTime;

            if (!input.Player.Attack2.IsPressed())
            {
                if (chgTimer > heavySlashChargeTime / 2) //lower power attack before full charge
                {
                    StartCoroutine(HeavySlash(0.5f + 0.25f * (chgTimer / heavySlashChargeTime)));
                    yield break;
                }
                else //cancel charge
                {
                    actionInProgress = false;
                    yield break;
                }
            }

            yield return null;
        }

        StartCoroutine(HeavySlash(1)); //full power charge attack
    }

    private IEnumerator HeavySlash(float chgAmount)
    {
        AttackScript atkScript = heavySlash.GetComponent<AttackScript>();

        atkScript.StartAttack(); //enable hitbox

        AttackBox atkBox = heavySlash.GetComponentInChildren<AttackBox>();
        float baseDmg = atkBox.damage;
        atkBox.damage *= chgAmount; //calculate damage

        float atkTimer = 0;

        while (atkTimer < heavySlashDuration)
        {
            atkTimer += Time.deltaTime;
            yield return null;
        }

        atkBox.damage = baseDmg; //reset damage of hitbox
        atkScript.EndAttack();

        yield return new WaitForSeconds(heavySlashCooldown); //ending lag

        actionInProgress = false;
    }

    //switch camera modes
    private void LockOn()
    {
        if (mainCamera == freeCam) //switch from free cam to lockon
        {
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
            yield return null;
        }
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