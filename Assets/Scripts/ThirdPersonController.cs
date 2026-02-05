using System.Collections;
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
    private float jumpDonwTime = 1f;

    [SerializeField]
    private float groundCheckPointRadius = 05f;

    [SerializeField]
    private LayerMask groundLayer;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        GroundCheck();
        //Debug.Log("grounded: " + isGrounded);
        Debug.Log(isAiming);

    }
    private void LateUpdate()
    {
        Look();
    }
    private void FixedUpdate()
    {
        Move();
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
    private IEnumerator JumpDownTimeCorutine()
    {
        yield return new WaitForSeconds(0.25f);

        var waitForGrounded = new WaitUntil(() => isGrounded);
        yield return waitForGrounded;

        yield return new WaitForSeconds(jumpDonwTime);
        canJump = true;
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
    private void OnAim(InputValue inputValue)
    {
        isAiming = inputValue.isPressed;
        
    }

    private void OnJump()
    {
        Jump();
    }
    private void OnRun(InputValue inputValue)
    {
        isRunning = inputValue.isPressed;
    }
    private void OnLook(InputValue inputValue)
    {
        look = inputValue.Get<Vector2>();
    }
    //private void GroundCheck()
    //{
    //    isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckPointRadius, groundLayer);
    //    animator.SetBool(groundedParamName, isGrounded);
    //}
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
    private void OnDrawGizmosSelected()
    {
        if(groundCheckPoint == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheckPoint.position, groundCheckPointRadius);
    }
    
}
