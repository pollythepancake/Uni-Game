using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    GameObject attackBox; //Attack collider object
    BoxCollider2D attackCollider;

    GameObject attackSprite; //Attack sprite
    SpriteRenderer sprite;

    GameObject jumpBox; //Jump collider object
    BoxCollider2D jumpCollider;

    GameObject UpgradeText; //Text object
    TextMesh UpgradeTextMesh;



    public PhysicsMaterial2D NormalPhysicsMat;
    public PhysicsMaterial2D WallJumpPhysicsMat;

    [Header("Movement")]
    public float walkSpeed = 2f; //Walking movement speed
    public float runMultiplier = 0.5f; //Running movement multiplier

    float moveSpeed; //Player movement speed



    [Header("Jumping")]
    public float maxJumps = 1f; //Max jumps the player can have
    public float jumpAmmount = 1f; //Ammount if jumps the player has
    public float jumpHeight = 8f; //Base jump Height
    public float doubleJumpHeight = 6f; //Double jump Height

    float height; //Jump height ammount



    [Header("Attacking")]
    public float AttackSize = 1f; 
    public float attackDelay = 1f;
    public float attackUpgradeSize = 1f;
    public float hitTime = 0.2f;

    bool delayed = false;

    [Header("Other")]
    float verInput; //Player vertical input
    float horInput; //Player horizontal input
    float facing; //Determins which way the player is facing



    //Start is called before the first frame update
    void Start()
    {
        //Get players RigidBody2D
        rb = GetComponent<Rigidbody2D>();

        //Get player colliders
        attackBox = GameObject.Find("AttackBox");
        attackCollider = attackBox.GetComponent<BoxCollider2D>();

        attackSprite = GameObject.Find("AttackSprite");
        sprite = attackSprite.GetComponent<SpriteRenderer>();

        jumpBox = GameObject.Find("JumpBox");
        jumpCollider = jumpBox.GetComponent<BoxCollider2D>();

        UpgradeText = GameObject.Find("UpgradeText");
        UpgradeTextMesh = UpgradeText.GetComponent<TextMesh>();

        sprite.enabled = false;

        UpgradeTextMesh.text = "";

        rb.sharedMaterial = NormalPhysicsMat;
    }



    private void FixedUpdate()
    {
        PlayerInput();
    }



    //Gets the players input
    private void PlayerInput()
    {
        //Player horizontal input
        horInput = Input.GetAxisRaw("Horizontal");

        //Player vertical input
        verInput = Input.GetAxisRaw("Vertical");


        if (verInput < 0) { facing = 2f; }
        else if (verInput > 0) { facing = 4f; }
        else if (horInput > 0) { facing = 3f; }
        else if (horInput < 0) { facing = 1f; }
    }



    //Update is called once per frame
    void Update()
    {
        //Move left and right
        if (Input.GetAxisRaw("Run") != 0) { moveSpeed = walkSpeed * runMultiplier; }
        else { moveSpeed = walkSpeed; }

        rb.velocity = new Vector2(horInput * moveSpeed, rb.velocity.y);

        //Jump
        if (Input.GetButtonDown("Jump") && jumpAmmount > 0)
        {
            if (maxJumps == 2f && jumpAmmount == 1) { height = doubleJumpHeight; }
            else { height = jumpHeight; }

            rb.velocity = new Vector2(rb.velocity.x, height);
            jumpAmmount -= 1;
        }
        //Attack

        if (Input.GetButtonDown("Fire1") && !delayed)
        {
            PlayerAttack();
        }
    }



    public void Upgrade(string upgrade)
    {
        if (upgrade == "Wall Jump Upgrade")
        {
            //Changes size of collider for wall jumps
            jumpCollider.offset = new Vector2(0, -0.1f);
            rb.sharedMaterial = WallJumpPhysicsMat;
            jumpCollider.size = new Vector2(1.2f, 1);
        }
        else if (upgrade == "Double Jump Upgrade")
        {
            //Changes number of jumps  
            maxJumps = 2f;
        }
        else if (upgrade == "Attack Length Upgrade")
        {
            //Changes dimensions of attack collider
            attackCollider.offset = new Vector2(attackCollider.offset.x, attackCollider.offset.y + attackUpgradeSize / 2);
            attackCollider.size = new Vector2(attackCollider.size.x, attackCollider.size.y + attackUpgradeSize);
            attackSprite.transform.localScale = new Vector2(attackSprite.transform.localScale.x + attackUpgradeSize, attackSprite.transform.localScale.y + attackUpgradeSize);
        }
        StartCoroutine(SetText(1, upgrade + " Acquired"));
    }

    //Text life
    IEnumerator SetText(float seconds, string text)
    {
        UpgradeTextMesh.text = text;
        yield return new WaitForSeconds(seconds);
        UpgradeTextMesh.text = "";
    }



    //Controllers player attacking
    public void PlayerAttack()
    {
        attackSprite.transform.localRotation = Quaternion.Euler(0, 0, facing * 90);
        attackBox.transform.localRotation = Quaternion.Euler(0, 0, facing * 90);
        StartCoroutine(AttackDelay(attackDelay));
        StartCoroutine(HitTime(hitTime));
    }

    //Delay between attacks
    IEnumerator AttackDelay(float seconds)
    {
        delayed = true;
        yield return new WaitForSeconds(seconds);
        delayed = false;
    }

    //Attack collider is active time
    IEnumerator HitTime(float seconds)
    {
        attackCollider.enabled = true;
        sprite.enabled = true;
        yield return new WaitForSeconds(seconds);
        attackCollider.enabled = false;
        sprite.enabled = false;
    }



    //Check if player is on the ground
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Solid")
        {
            jumpAmmount = maxJumps;
        }

        if (collision.gameObject.tag == "Upgrade")
        {
            Upgrade(collision.gameObject.name);
            Destroy(collision.gameObject);
        }
    }
}
