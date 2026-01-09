using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour {
    private float horizontalMove;
    private float verticalMove;
    public CharacterController player;
    public float playerSpeed; 
    // Use this for initialization
    void Start () {
        player = GetComponent<CharacterController>();
    }
    
 
    void Update () {
        //old input sistem
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");

    }
    private void FixedUpdate()
    {
        player.Move(new Vector3(horizontalMove, 0, verticalMove) * playerSpeed * Time.deltaTime);
    }
}