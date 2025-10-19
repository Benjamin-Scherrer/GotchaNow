using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIntermission : MonoBehaviour
{
    public static PlayerIntermission Instance;
    private InputSystem_Actions input = null;
    [HideInInspector] public Rigidbody rb = null;
    public GameObject model;
    private Camera mainCamera;
    public Camera freeCam;
    private Vector2 moveVector = Vector2.zero;
    private Vector2 camVector = Vector2.zero;
    private Vector2 stickPosition;
    [HideInInspector] public bool actionInProgress = false;
    public float moveSpeed = 5;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        mainCamera = freeCam;

        input = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();

    }
    
    void Start()
    {

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

    // Update is called once per frame
    void Update()
    {
        //check left analog
        stickPosition = new Vector2(moveVector.x, moveVector.y);
    }

    void FixedUpdate()
    {
        moveCharacter(stickPosition);
    }

    private void moveCharacter(Vector2 direction) //movement
    {
        Vector3 camFwd = new Vector3(transform.position.x - mainCamera.transform.position.x, 0, transform.position.z - mainCamera.transform.position.z);
        float rotationY = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Vector3 moveDir = Quaternion.Euler(0, rotationY, 0) * camFwd.normalized;

        float tiltStrength = direction.magnitude; //analog movement speed

        if (!actionInProgress && direction.magnitude > 0.1f)
        {
            rb.MovePosition(rb.position + moveDir * tiltStrength * moveSpeed * Time.fixedDeltaTime);
            transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + moveDir, 0.25f));

            /* if (lockedOn) //rotate towards lock on target
            {
                Vector3 targetPos = lockOnTarget.GetComponent<LockOnTarget>().targetEnemy.transform.position;
                transform.LookAt(Vector3.Lerp(transform.position + transform.forward, new Vector3(targetPos.x, transform.position.y, targetPos.z), 0.15f));
            } */
            
        }
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }
    private void OnMovementCanceled(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;
    }
}
