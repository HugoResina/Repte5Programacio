using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
public class PlayerController : MonoBehaviour ,InputSystem_Actions.IPlayerActions
{
    private float horizontalMove;
    private float verticalMove;
    public CharacterController controller;
    public float playerSpeed; 
    private Animator animator;
    InputSystem_Actions inputActions;

    private Vector2 lookInput;
    private Vector2 moveInput;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.SetCallbacks(this);
    }
    void Start () {

        controller = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();

      
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
    private void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * playerSpeed * Time.deltaTime);
        

        animator.SetBool("IsWalking", moveInput != Vector2.zero);
    }

  

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Debug.Log("look");
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}