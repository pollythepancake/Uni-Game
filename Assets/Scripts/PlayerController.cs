using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Movement")]
    public float walkSpeed; //Walking movement speed
    public float runSpeed; //Running movement multiplier
    float moveSpeed; //Player movement speed
    float moveVelocity; //Players movement velocity

    [Header("Jumping")]
    public float jumpHeight; //Jump Height
    float jumpVelocity; //Players jump velocity
    bool grounded = true; //If player is on the ground

    // Start is called before the first frame update
    void Start()
    {
        // Get players RigidBody2D
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move left and right
        moveVelocity = Input.GetAxisRaw("Horizontal");

        if (Input.GetAxisRaw("Run") != 0) { moveSpeed = walkSpeed * runSpeed; } //If run button pressed
        else { moveSpeed = walkSpeed; }

        rb.velocity = new Vector2(moveVelocity * moveSpeed, rb.velocity.y);

        //Jump up
        jumpVelocity = Input.GetAxisRaw("Vertical");
        if (Input.GetAxisRaw("Jump") != 0 && grounded == true)
        { 
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            grounded = false;
        }
    }

    //Check if player is on the ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        grounded = true;
    }
}
