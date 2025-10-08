using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;
using UnityEssentials;

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
    [HideInInspector] public bool attack1Active = false;
    [HideInInspector] public bool combo2Active = false;
    [HideInInspector] public bool combo3Active = false;
    [HideInInspector] public bool dodgeActive = false;
    [HideInInspector] public bool guardActive = false;
    [HideInInspector] public bool parryActive = false;

    //hitbox objects
    public GameObject attackStartup;
    public GameObject attack1Box;
    public GameObject combo2Box;
    public GameObject combo3Box;
    public GameObject parrySphere;

    private bool attack1Ready = true;
    private bool attack1Charge = false;
    private bool combo2Queued = false;

    public float attack1Cooldown = 1f;
    public float attack1StartTime = 0.5f;
    public float attack1ActiveTime = 0.5f;

    public float combo2ActiveTime = 0.5f;
    public float combo3ActiveTime = 0.5f;

    public float attack1fMovement = 1f;
    public float combo2fMovement = 1f;
    public float combo3fMovement = 1f;
    public float attackRotationInfluence = 3f;

    private bool dodgeReady = true;
    public float dodgeCooldown = 1f;
    private float dodgeTimer = 0f;
    private Vector3 dodgeDir;
    [HideInInspector] public bool dodgeSuccessful = false;

    private bool parryReady = true;
    public float parryLength = 0.1f;
    public float parryCooldown = 0.3f;
    private float parryTimer = 0f;
    [HideInInspector] public bool parrySuccessful = false;

    private Vector3 attackBoxSize;
    private Vector3 comboBoxSize;

    //colors
    public Material defaultMaterial;
    public Material dodgeMaterial;
    public Material guardMaterial;
    public Material hitstunMaterial;

    /* private GameObject VFXBlur;
    private GameObject HitBloom; */

    private void Awake()
    {
        Instance = this;

        mainCamera = freeCam;

        input = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();

        attackBoxSize = attack1Box.transform.localScale;
        //comboBoxSize = combo2Box.transform.localScale;

        //GetComponent<MeshRenderer>().material = defaultMaterial;

        //parrySphere.SetActive(false);

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
        if (!attack1Ready && !attack1Charge && !attack1Active)
        {
            if (!input.Player.Attack1.IsPressed())
            {
                attack1Ready = true;
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
            }
            else if (lockedOn == true)
            {
                lockedOn = false;
                LockOn();
            }

            lockOnReady = false;
        }

        if (input.Player.LockOnR.IsPressed() && switchReadyR)
            {
                Debug.Log("switch R");
                lockOnCam.GetComponent<LockOnCamera>().SwitchTargetR();
                switchReadyR = false;
            }

        if (input.Player.LockOnL.IsPressed() && switchReadyL)
            {
                Debug.Log("switch L");
                lockOnCam.GetComponent<LockOnCamera>().SwitchTargetL();
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
        if (input.Player.Attack1.IsPressed() && attack1Ready && !actionInProgress && !hitStun)
        {
            /* actionInProgress = true;

            attackStartup.SetActive(true);
            attackStartup.transform.localPosition = new Vector3(0.75f, 0, -0.25f);
            projectileAttackCharge = true;
            projectileAttackReady = false; */
        }

        //parry cooldown
        if (parryReady == false)
        {
            parryCooldown -= Time.deltaTime;

            //re-enable parry after cooldown & letting go of shield button
            if (parryCooldown <= 0 && !input.Player.Block.IsPressed())
            {
                parryReady = true;
            }
        }

        //input block
        if (input.Player.Block.IsPressed() && !actionInProgress && !hitStun)
        {
            guardActive = true;

            if (parryReady)
            {
                /* parryTimer = parryLength;
                parrySphere.SetActive(true);
                parryActive = true;
                parryReady = false;
                parryCooldown = 0.33f; */
            }
        }

        //parry active
        if (parryActive)
        {
            parryTimer -= 1 * Time.fixedDeltaTime;
            actionInProgress = true;

            if (parrySuccessful)
            {
                //HitBloom.gameObject.GetComponent<HitBloom>().hitCheck = true;
                //ParrySFX audio;

                parrySuccessful = false;
            }

            if (parryTimer <= 0)
            {
                actionInProgress = false;
                parryActive = false;
                //parrySphere.SetActive(false);
            }
        }

        //block active
        if (guardActive)
        {
            if (!input.Player.Block.IsPressed())
            {
                guardActive = false;
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

                actionInProgress = false;
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

            transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.5f));
        }
    }

    private void DodgeRoll(Vector3 direction) //dodge movement
    {
        Vector3 camFwd = new Vector3(transform.position.x - mainCamera.transform.position.x, 0, transform.position.z - mainCamera.transform.position.z);
        rotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Vector3 moveDir = Quaternion.Euler(0, rotationY, 0) * camFwd.normalized;

        rb.linearVelocity = moveDir * dodgeSpeed;
    }

    public void SlashAttack()
    {
        /* int repeatCount = playerStats.repeatMod + 1;
        int spreadCount = playerStats.spreadMod + 1;

        playerStats.repeatMod = 0;
        playerStats.spreadMod = 0;

        for (int i = 0; i < repeatCount; i++)
        {
            if (i % 2 == 0)
            {
                attack1Box.SetActive(true);
                attack1Box.GetComponentInChildren<Attack1Script>().slashLvl = slashLvl;
                attack1Box.transform.localPosition = new Vector3(0.8f, 0, 1);
                attack1Box.transform.localScale = new Vector3(attackBoxSize.x * (1 + spreadCount / 2f), attackBoxSize.y, attackBoxSize.z * (1 + spreadCount / 4));

                while (attack1Box.transform.localPosition.x > -0.8)
                {
                    attack1Box.transform.localPosition += new Vector3(-1, 0, 0) * (0.5f*repeatCount) * attack1ActiveTime * Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                attack1Box.transform.localScale = attack1Box.GetComponentInChildren<Attack1Script>().originalSize;
                attack1Box.SetActive(false);
            }
            else if (i % 2 == 1)
            {
                combo2Box.SetActive(true);
                combo2Box.GetComponentInChildren<Attack1Script>().slashLvl = slashLvl;
                combo2Box.transform.localPosition = new Vector3(-0.8f, 0, 1);
                combo2Box.transform.localScale = new Vector3(comboBoxSize.x * (1 + spreadCount / 2f) * (1 + 0.1f * slashLvl), comboBoxSize.y, comboBoxSize.z * (1 + spreadCount / 4) * (1 + 0.1f * slashLvl));

                while (combo2Box.transform.localPosition.x < 0.8)
                {
                    combo2Box.transform.localPosition += new Vector3(1, 0, 0) * (0.5f*repeatCount) * combo2ActiveTime * Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                combo2Box.transform.localScale = combo2Box.GetComponentInChildren<Attack1Script>().originalSize;
                combo2Box.SetActive(false);
            }
        } */
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