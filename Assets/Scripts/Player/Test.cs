using Unity.VisualScripting;
using UnityEngine;
/* 
control everything about player 
*/

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerControl : MonoBehaviour{ 
    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private Animator animator;
    private SpriteRenderer sprite;
    private Vector2 frameVelocity;
    private float time; // Global timer
    private bool phsic; // store the state of Physics2D.queriesStartInColliders
    private float moveInput;

    public LayerMask PlayerLayer;

    [Header("Move")]
    public float Maxspeed = 10;
    public float acceleration = 40;
    public float groundDeceleration = 40;
    public float airDeceleration = 20;
    public float GrounderDistance = 0.05f;

    [Header("Jump")]
    public float jumpForce = 18f;
    public float groundingForce = -1.5f;
    public float jumpDeceleration = 40;
    public float maxFallSpeed = 40;
    public float jumpEndedEarlyGravity = 90;
    public float bufferJump = .2f;
    public float coyoteTime = .1f;

    [Header("Apex modifiers")]
    public float apexFallSpeed = 10;
    public float apexGravityDuraction = 0.15f;
    public float apexFallAcceleration = 10;

    [Header("Dead")]
    public float masFallTime = 6;

    /* 
    set everything once the game start 
    */ 
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        col = GetComponent<CapsuleCollider2D>();

        animator = GetComponent<Animator>();

        sprite = GetComponent<SpriteRenderer>();

        phsic = Physics2D.queriesStartInColliders;
    }


    /* 
    put everything about input in the Update 
    because it cheaks per frame it runs at the same rate as the game's frame rate
    */
    private void Update()
    {
        GatherInput();
        DieAndRespawn();
        time += Time.deltaTime; // keep adding to the timer

        float horizontalInput = Input.GetAxis("Horizontal"); // get the input
            animator.SetFloat("speed", Mathf.Abs(horizontalInput)); // set speed

            moveInput = horizontalInput;

        if(isFalling){
            fallTimer += Time.deltaTime;
        }else {
            fallTimer = 0;
        }
    }


    /* 
    put everything about physics in the FixedUpdate 
    because it can ensure accuracy and consistency in physics calulations 
    */ 
    private void FixedUpdate()
    {
         // apex timer, to end the apexMonment. use the Mathf.Max to make sure the spexTimer won't go too low
        apexTimer = Mathf.Max(apexTimer -= Time.fixedDeltaTime, 0);

        HandleMovement();
        HandleJump();
        CheckCollison();
        HandleGravity();
        ApplyMovement();

        // Flip character direction
        if (moveInput > 0)
        {
            sprite.flipX = false; // Moving right, no flip
        }
        else if (moveInput < 0)
        {
            sprite.flipX = true; // Moving left, flip
        }
    }

    private float horizontal;


    /* 
    Cheak everthing about Input like Horizontal, Jump, and j key 
    change the value of the variable after the jump down
    */
    private void GatherInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal"); // Input for horizontal movement
        jumpDown = Input.GetButtonDown("Jump"); // Detect if the jump button is pressed
        jumpHeld = Input.GetButton("Jump"); // Detect if the jump button is held down

        if (jumpDown){
            jumpToConsume = true;
            timeJumpPressed = time;
            bufferJumpUsable = true;
        }

    }


    /* 
    Handle the Horizontal movement 
    */
    private void HandleMovement()
    {
        // use the groundDeceleration if on the ground, use the airDecerleration if not on the ground
        var deceleration = grounded ? groundDeceleration : airDeceleration; 
        if (horizontal == 1){
            frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, Maxspeed, acceleration * Time.fixedDeltaTime);
        } else if(horizontal == -1){
            frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, -Maxspeed, acceleration * Time.fixedDeltaTime);
        } else if (horizontal == 0){
            frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        } // Make acceleration and deceleration look more natural with the Mathf.MoveTowards to 
    }


    /* 
    check if the player is on the ground or hit the ceiling by the collider 
    change the value of the grounded to check if the player can jump 
    */ 
    private void CheckCollison()
    {
        Physics2D.queriesStartInColliders = false; // Disabled the Physics2D.queriesStartInColliders

        // Cast a line downward from the center of the character to check if it is on the ground
        bool groundHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.down, GrounderDistance, ~PlayerLayer);
        //  Cast a line upward from the center of the character to check if it is on the ground
        bool ceilingHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.up, GrounderDistance, ~PlayerLayer);

        if(ceilingHit) {
            frameVelocity.y = Mathf.Min(0, frameVelocity.y);
        } // if hit the ceiling then stop moving up

        if(grounded && !groundHit){ // if leave the ground
            grounded = false;
            bufferJumpUsable = true;
            timeLeftGround = time;
        } else if(!grounded && groundHit){// if land on the gorund
            grounded = true;
            jumpEndedEarly = false;
            coyoteUsable = true;
            isJumping = false;
        }

        Physics2D.queriesStartInColliders = phsic;
    }

    // Synchronize Rigidbody with frameVelocity
    private void ApplyMovement() => rb.linearVelocity = frameVelocity;

     private bool grounded;
    private bool jumpDown;
    private bool jumpHeld;
    private bool jumpToConsume;
    private float timeJumpPressed;
    private float timeLeftGround;
    private bool jumpEndedEarly;
    private bool coyoteUsable;
    private bool bufferJumpUsable;
    private bool CanUseBufferJump => bufferJumpUsable && time < timeJumpPressed + bufferJump;
    private bool CanUseCototaTime => coyoteUsable && !grounded && time < timeLeftGround + coyoteTime;


    /* 
    check should end the jump early 
    check if player press the jump button  
    if yes, then check if player can jump 
    if yes call ExecuteJump 
    */
    private void HandleJump()
    {
        // if not hold the space during the jumping, then jumpEndedEarly
        if(!jumpEndedEarly && !grounded && !jumpHeld && rb.linearVelocity.y > 0) jumpEndedEarly = true; 

        if(!jumpToConsume && !CanUseBufferJump) return; // if either of them is false, check the next one

        if(grounded || CanUseCototaTime) ExecuteJump(); // if either of them is true, execute the jump

        jumpToConsume = false;
    }


    /*
    when handle jumpcalled, exxcute the jump
    */
    private void ExecuteJump()
    {
        frameVelocity.y = jumpForce;
        jumpEndedEarly = false;
        bufferJumpUsable = false;
        coyoteUsable = false;
        isJumping = true;
        timeJumpPressed = 0;
    }

    private bool isJumping;
    private bool inApexMonment;
    private float apexTimer;


     /* 
    change the gravity for the different time like on the ground or jump or end the jump early or apex time 
    */ 
    private void HandleGravity()
    {
        var inAirGravity = maxFallSpeed; // use this variable to change the gravity if jumpEnddedEarly
        if(jumpEndedEarly && frameVelocity.y > 0) inAirGravity = jumpEndedEarlyGravity;
        var storeGravity = inAirGravity; // Store the state of the inAirGravity now

        // apply the apex, and reduce the gravity at the top
        if(isJumping && !inApexMonment && Mathf.Abs(frameVelocity.y) < 0.5 && !jumpEndedEarly){ 
            inApexMonment = true;
            apexTimer = apexGravityDuraction;
            inAirGravity = Mathf.MoveTowards(inAirGravity, apexFallSpeed, apexFallAcceleration);   
        }
        if(inApexMonment){
            if(apexTimer <= 0){
                inAirGravity = Mathf.MoveTowards(inAirGravity, storeGravity, apexFallAcceleration);
                inApexMonment = false;
            } // end the apexMonment after the apexTimer is 0
        }

        if(grounded && frameVelocity.y <= 0 ){ // Apply ground gravity when not jumping
            frameVelocity.y = groundingForce;
        } else { // Apply jump gravity during the jump
            frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, -jumpDeceleration, inAirGravity * Time.fixedDeltaTime);
        } 
    }

    private float fallTimer;
    private bool isFalling;

    // 
    private void DieAndRespawn()
    {
        if(!grounded && frameVelocity.y <= 0){
        isFalling = true;
        } else {
            isFalling = false;
        }

        if(fallTimer >= masFallTime){
            transform.position = new Vector2(-23, 0);
            fallTimer = 0;
        }
    }
}