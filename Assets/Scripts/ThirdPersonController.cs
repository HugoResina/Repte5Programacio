using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    private const string speedParamName = "Speed";
    private const string jumpParamName = "Jump";
    private const string groundedParamName = "Grounded";
    private const string fallingParamName = "Falling";
    private const string aimingParamName = "Aim";
    private const string danceParamName = "Dance";
    private const float lookThreshold = 0.01f;
    

    [Header("Cinemachine")]
    [SerializeField]
    private Transform cameraTarget;

    [SerializeField]
    private float topClamp = 70f;

    [SerializeField]
    private float bottomClamp = -30f;


    [Header("Speed")]
    [SerializeField]
    private float lookSpeed = 10f;

    [SerializeField]
    private float moveSpeed = 3f;
    [Header("Grounded")]
    [SerializeField]
    private Transform groundCheckPoint;

    [Header("Jump")]
    [SerializeField]
    private float jumpStrength = 7f;

    [SerializeField]
    private float jumpDownTime = 1f;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private ParticleSystem muzzleFlash;

   

    private Rigidbody rb;
    private Animator animator;
    private Vector2 move;
    private Vector2 look;
    private float currentSpeed;      
    private float yaw;
    private float pitch;
    private bool isRunning;
    private bool isGrounded = true;
    private bool canJump = true;
    private bool isAiming = false;
    private bool isDancing = false;

    public GameObject Spine;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    private void Update()
    {
        GroundCheck();
        //Debug.Log("grounded: " + isGrounded);
        //Debug.Log(isAiming);


    }
    private void LateUpdate()
    {

        if(!isDancing) Look();
    }
    private void FixedUpdate()
    {
        if (!isDancing) Move();
    }
    private void Jump()
    {
        if(!isGrounded || !canJump)
        {
            return;
        }

        rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        canJump = false;
        StartCoroutine(JumpDownTimeCorutine());
        animator.SetTrigger(jumpParamName);
    }
    private void OnDance()
    {
        if (!isDancing && isGrounded)
        {
            animator.SetFloat(speedParamName, 0.001f);
            //Debug.Log("ballo");
            animator.SetBool(danceParamName, true);
            isDancing = true;

           
            StartCoroutine(DanceFinnishCorutine());
        }

    }
    private IEnumerator JumpDownTimeCorutine()
    {
        yield return new WaitForSeconds(0.25f);

        var waitForGrounded = new WaitUntil(() => isGrounded);
        yield return waitForGrounded;

        yield return new WaitForSeconds(jumpDownTime);
        canJump = true;
    }
    private IEnumerator DanceFinnishCorutine()
    {
        yield return new WaitForSeconds(10);
        isDancing = false;
        animator.SetBool(danceParamName, false);
    }
    //private void Move()
    //{
    //    float targetSpeed = (isRunning ? moveSpeed * 2f : moveSpeed) * move.magnitude;
    //    currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * 8f);

    //    Vector3 forward = cameraTarget.forward;
    //    Vector3 right = cameraTarget.right;

    //    forward.y = 0f;
    //    right.y = 0f;
    //    forward.Normalize();
    //    right.Normalize();

    //    Vector3 moveDirection = (forward * move.y + right * move.x).normalized;

    //    if(moveDirection.sqrMagnitude > 0.01f)
    //    {
    //        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);

    //        Vector3 currentVelocity = rb.linearVelocity;
    //        rb.linearVelocity = new Vector3(moveDirection.x * currentSpeed, moveDirection.y * currentSpeed, moveDirection.z * currentSpeed);
    //    }
    //    else
    //    {
    //        Vector3 currentVelocity = rb.linearVelocity;
    //        rb.linearVelocity = new Vector3(0, currentVelocity.y, 0);
    //    }

    //    float normalizedAnimSpeed = currentSpeed / (moveSpeed * 2f);
    //    animator.SetFloat(speedParamName, normalizedAnimSpeed);
    //    animator.SetBool(fallingParamName, !isGrounded && rb.linearVelocity.y < -0.1f);
    //}
    private void Move()
    {
        if(animator.GetFloat(speedParamName) > 0.2f)
        {
            animator.SetBool(aimingParamName, false);
            AimController.AimContInstance.isAiming = false;
            isAiming = false;


        }
        float targetSpeed = (isRunning ? moveSpeed * 2f : moveSpeed) * move.magnitude;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * 8f);

        Vector3 forward = cameraTarget.forward;
        Vector3 right = cameraTarget.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * move.y + right * move.x).normalized;

        Vector3 velocity = rb.linearVelocity;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.fixedDeltaTime * 10f
            );

            velocity.x = moveDirection.x * currentSpeed;
            velocity.z = moveDirection.z * currentSpeed;
        }
        else
        {
            velocity.x = 0f;
            velocity.z = 0f;
        }

       
        rb.linearVelocity = velocity;

        float normalizedAnimSpeed = currentSpeed / (moveSpeed * 2f);
        animator.SetFloat(speedParamName, normalizedAnimSpeed);

        animator.SetBool(
            fallingParamName,
            !isGrounded && rb.linearVelocity.y < 0.1f
        );
    }
    private void Look()
    {
        if(look.sqrMagnitude > lookThreshold)
        {
            float deltaMultiplier = Time.deltaTime * lookSpeed;
            yaw += look.x * deltaMultiplier;
            pitch -= look.y * deltaMultiplier;
        }
        yaw = ClampAngle(yaw, float.MinValue, float.MaxValue);
        pitch = ClampAngle(pitch, bottomClamp, topClamp);

        cameraTarget.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        if (isAiming)
        {
            float yRotation = cameraTarget.transform.eulerAngles.y;
           

            rb.rotation = Quaternion.Euler(0f, yRotation, 0f);
           
            float xRotation = cameraTarget.transform.localEulerAngles.x;
           
            if (xRotation > 180f)
                xRotation -= 360f;

            xRotation = Mathf.Clamp(xRotation, -40f, 40f);

            Vector3 spineRotation = Spine.transform.localEulerAngles;

            spineRotation.x = xRotation*0.5f;

            Spine.transform.localEulerAngles = spineRotation;
            
        }
    }

    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if(lfAngle < -360f)
        {
            lfAngle += 360f;
        }
        if(lfAngle > 360f)
        {
            lfAngle -= 360f;
        }

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnMove(InputValue inputValue)
    {
        move = inputValue.Get<Vector2>();
    }
    public void OnAim(InputValue value)
    {
        isAiming = !animator.GetBool(aimingParamName) && animator.GetFloat(speedParamName) < 0.2f && isGrounded && !isDancing;
        AimController.AimContInstance.isAiming = isAiming;

        animator.SetBool(aimingParamName, isAiming);
    }
    
    public void OnAttack(InputValue inputValue)
    {
        if (isAiming)
        {
            if(muzzleFlash != null) muzzleFlash.Play();
            //Debug.Log("pam");
            
        }
    }
   
    private void OnJump()
    {
        if (!isDancing)
        {
            Jump();
            animator.SetBool(aimingParamName, false);
        }

        
    }
    private void OnRun(InputValue inputValue)
    {
        isRunning = inputValue.isPressed;
    }
    private void OnLook(InputValue inputValue)
    {
        look = inputValue.Get<Vector2>();
    }
 
    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            0.3f,
            groundLayer
        );

        animator.SetBool(groundedParamName, isGrounded);
    }
   
    
}
