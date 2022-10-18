using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MouseFollow
{
    public Rigidbody2D body;
    public Animator anim;
    public Transform interactor; // Shows mouse position
    public Transform interactor_two; // Shows what user inputted
    public float walkSpeed;
    public playerAction playerControls;
    public Vector2 direction;

    private float inputX;
    private float inputY;

    // Update is called once per frame
    void Update() {

        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        direction = new Vector2(inputX, inputY).normalized;
        
        anim.SetFloat("Horizontal", inputX);
        anim.SetFloat("Vertical", inputY);
        anim.SetFloat("Speed", direction.sqrMagnitude);

        //give the game info of the direction that the player is facing (for interaction feature later)
    
        if(Input.GetAxis("Horizontal") == 1 || Input.GetAxis("Horizontal") == -1 || Input.GetAxis("Vertical") == 1 || Input.GetAxis("Vertical") == -1) {
            anim.SetFloat("LastHorizontal", Input.GetAxis("Horizontal"));
            anim.SetFloat("LastVertical", Input.GetAxis("Vertical"));
        }

        
        if (inputX == 0 && inputY > 0) /*N*/ {
            interactor_two.localRotation = Quaternion.Euler(0, 0, 180);
        }
        if (inputX > 0 && inputY > 0) /*NE*/ {
            interactor_two.localRotation = Quaternion.Euler(0, 0, 135);
        }
        if (inputX > 0 && inputY == 0) /*E*/ {
            interactor_two.localRotation = Quaternion.Euler(0, 0, 90);
        }
        if (inputX > 0 && inputY < 0) /*SE*/ {
            interactor_two.localRotation = Quaternion.Euler(0, 0, 45);
        }
        if (inputX == 0 && inputY < 0) /*S*/ {
            interactor_two.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (inputX < 0 && inputY < 0) /*SW*/ {
            interactor_two.localRotation = Quaternion.Euler(0, 0, -45);
        }
        if (inputX < 0 && inputY == 0) /*W*/ {
            interactor_two.localRotation = Quaternion.Euler(0, 0, -90);
        }
        if (inputX < 0 && inputY > 0) /*NW*/ {
            interactor_two.localRotation = Quaternion.Euler(0, 0, -135);
        }

        // Turns the interactor towards the mouse

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        interactor.localRotation = Quaternion.LookRotation(Vector3.forward, interactor.position - mousePosition); 
        
    }

    void FixedUpdate() {  
        body.velocity = new Vector2(direction.x * walkSpeed, direction.y * walkSpeed);
    }

    private void OnEnable(){
        playerControls.Player.Enable();
    }
    private void OnDisable(){
        playerControls.Player.Disable();

    }
}