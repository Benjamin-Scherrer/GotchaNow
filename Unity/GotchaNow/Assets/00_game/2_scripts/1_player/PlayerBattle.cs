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
    public float maxHealth = 100;
    public float health = 100;
    public float moveSpeed = 0.5f;
    public float dodgeSpeed = 5f;
    public float rotateSpeed = 10f;
    [HideInInspector] public bool death = false;
    public float deathCountdown = 0f;

    [HideInInspector] public bool hitStun = false;
    [HideInInspector] public float hitStunTimer = 0f;

    //public so enemies can check player state
    [HideInInspector] public bool actionInProgress = false;
    [HideInInspector] public bool slash1Active = false;
    [HideInInspector] public bool slash2Active = false;
    [HideInInspector] public bool slash3Active = false;
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

    public float slashCooldown = 1f;
    public float slash1Duration = 0.5f;
    public float slash2Duration = 0.5f;
    public float slash3Duration = 0.5f;

    public float slash2Window = 0.25f;
    public float slash3Window = 0.25f;
    
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
    public Material guardMaterial;
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
        input.Player.CameraControl.performed += OnCameraPerformed;
        input.Player.CameraControl.canceled += OnCameraCanceled;
    }

    private void OnDisable()
    {
        //input system stuff
        input.Disable();
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCanceled;
        input.Player.CameraControl.performed -= OnCameraPerformed;
        input.Player.CameraControl.canceled -= OnCameraCanceled;
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
        if (health <= 0)
        {
            if (death) return;
            death = true;
            GetComponent<MeshRenderer>().enabled = false;
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

        //attack
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
        if (hitStun)
        {
            hitStunTimer -= Time.deltaTime;
            //GetComponent<MeshRenderer>().material = hitstunMaterial;

            if (hitStunTimer <= 0)
            {
                //GetComponent<MeshRenderer>().material = defaultMaterial;
                hitStun = false;
            }
        }

        //attack
        if (input.Player.Attack1.IsPressed() && slashReady && !actionInProgress && !hitStun)
        {
            actionInProgress = true;
            slashReady = false;
            StartCoroutine(Slash1());
        }

        //heavy attack
        if (input.Player.Attack2.IsPressed() && slashReady && !actionInProgress && !hitStun)
        {
            /* actionInProgress = true;
            slashReady = false;
            StartCoroutine(HeavySlash()); */
        }

        //input block
        if (input.Player.Block.IsPressed() && blockReady && !actionInProgress && !hitStun)
        {
            guardActive = true;
            blockReady = false;
            blockBox.GetComponent<BlockScript>().StartBlock();
        }

        //block active
        if (guardActive)
        {
            if (!input.Player.Block.IsPressed())
            {
                guardActive = false;
                blockBox.GetComponent<BlockScript>().EndBlock();
            }
        }

        //input dodge
        if (input.Player.Dodge.IsPressed() && dodgeReady && !actionInProgress && !hitStun)
        {
            actionInProgress = true;

            dodgeDir = stickPosition;
            dodgeActive = true;
            dodgeReady = false;
            dodgeTimer = dodgeCooldown;

            //DodgeSFX audio;
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

    private void moveCharacter(Vector3 direction) //movement
    {
        Vector3 camFwd = new Vector3(transform.position.x - mainCamera.transform.position.x, 0, transform.position.z - mainCamera.transform.position.z);
        rotationY = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg;
        Vector3 moveDir = Quaternion.Euler(0, rotationY, 0) * camFwd.normalized;

        float tiltStrength = direction.magnitude;

        if (!actionInProgress && !hitStun && direction.magnitude > 0.1f)
        {
            if (guardActive)
            {
                rb.MovePosition(rb.position + moveDir * tiltStrength * moveSpeed * 0.33f * Time.fixedDeltaTime); //slow down when blocking
            }
            else
            {
                rb.MovePosition(rb.position + moveDir * tiltStrength * moveSpeed * Time.fixedDeltaTime);
            }

            if (lockedOn)
            {
                Vector3 targetPos = lockOnTarget.GetComponent<LockOnTarget>().targetEnemy.transform.position;
                transform.LookAt(Vector3.Lerp(transform.position + transform.forward, new Vector3(targetPos.x, transform.position.y, targetPos.z), 0.15f));
            }
            else
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

    private IEnumerator Slash1()
    {
        slash1.GetComponent<AttackScript>().StartAttack();
        float atkTimer = 0;

        while (atkTimer < slash1Duration)
        {
            atkTimer += Time.deltaTime;

            if (atkTimer > slash1Duration - slash2Window)
            {
                if (input.Player.Attack1.IsPressed() && slashReady && !dodgeQueued)
                {
                    slash2Queued = true;
                    slashReady = false;
                }
                else if (input.Player.Dodge.IsPressed() && dodgeReady && !slash2Queued)
                {
                    dodgeQueued = true;
                    dodgeReady = false;
                    atkTimer *= 1.5f;
                }
            }

            yield return null;
        }

        slash1.GetComponent<AttackScript>().EndAttack();

        if (slash2Queued)
        {
            StartCoroutine(Slash2());
            slash2Queued = false;
            yield break;
        }
        else if (dodgeQueued)
        {
            dodgeDir = stickPosition;
            dodgeActive = true;
            dodgeTimer = dodgeCooldown;

            dodgeQueued = false;
            yield break;
        }

        yield return new WaitForSeconds(slashCooldown);

        actionInProgress = false;
    }

    private IEnumerator Slash2()
    {
        slash2.GetComponent<AttackScript>().StartAttack();
        float atkTimer = 0;

        while (atkTimer < slash2Duration)
        {
            atkTimer += Time.deltaTime;

            if (atkTimer > slash2Duration - slash3Window)
            {
                if (input.Player.Attack1.IsPressed() && slashReady && !dodgeQueued)
                {
                    slash3Queued = true;
                    slashReady = false;
                }
                else if (input.Player.Dodge.IsPressed() && dodgeReady && !slash2Queued)
                {
                    dodgeQueued = true;
                    dodgeReady = false;
                    atkTimer *= 1.5f;
                }
            }
            
            yield return null;
        }

        slash2.GetComponent<AttackScript>().EndAttack();

        if (slash3Queued)
        {
            StartCoroutine(Slash3());
            slash3Queued = false;
            yield break;
        }
        else if (dodgeQueued)
        {
            dodgeDir = stickPosition;
            dodgeActive = true;
            dodgeTimer = dodgeCooldown;

            dodgeQueued = false;
            yield break;
        }

        yield return new WaitForSeconds(slashCooldown);

        actionInProgress = false;
    }

    private IEnumerator Slash3()
    {
        slash3.GetComponent<AttackScript>().StartAttack();
        float atkTimer = 0;

        while (atkTimer < slash3Duration)
        {
            atkTimer += Time.deltaTime;
            yield return null;
        }

        slash3.GetComponent<AttackScript>().EndAttack();

        yield return new WaitForSeconds(slashCooldown);
        
        actionInProgress = false;
    }

    private void LockOn()
    {
        if (mainCamera == freeCam)
        {
            lockOnCam.SetActive(true);
            //lockOnCam.GetComponent<Camera>().enabled = true;
            lockOnCam.GetComponent<LockOnCamera>().isActive = true;
            lockOnCam.GetComponent<LockOnCamera>().SetTarget();

            mainCamera = lockOnCam;

            mainCamera.transform.position = freeCam.transform.position;
            mainCamera.transform.rotation = freeCam.transform.rotation;

            freeCam.SetActive(false);

            //freeCam.GetComponent<Camera>().enabled = false;
        }

        else if (mainCamera == lockOnCam)
        {
            freeCam.SetActive(true);
            //freeCam.GetComponent<Camera>().enabled = true;
            lockOnCam.GetComponent<LockOnCamera>().isActive = false;

            mainCamera = freeCam;

            mainCamera.GetComponent<CinemachineOrbitalFollow>().ForceCameraPosition(lockOnCam.transform.position, lockOnCam.transform.rotation);

            lockOnCam.SetActive(false);
            
            //lockOnCam.GetComponent<Camera>().enabled = false;
        }
    }

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

    //input system stuff for right analog movement
    private void OnCameraPerformed(InputAction.CallbackContext value)
    {
        camVector = value.ReadValue<Vector2>();
    }

    private void OnCameraCanceled(InputAction.CallbackContext value)
    {
        camVector = Vector2.zero;
    }
}