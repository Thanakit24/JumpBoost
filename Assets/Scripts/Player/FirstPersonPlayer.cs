 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonPlayer : MonoBehaviour
{
    public static FirstPersonPlayer instance;
    public MovementStates state;
    public enum MovementStates 
    {
        walking,
        sprinting,
        air,
        fastfall,
        wallrunning
    }

    [Header("Movement")]
    public float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float wallrunSpeed;
    public Transform orientation;

    public bool wallRunning;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    public float playerHeight = 1.5f;

    [Header("Jump")]
    public float defaultJumpForce;
    public float currentJumpForce;
    public float maxJumpForce;
    public float maxAirVelocity;
    public float airMultiplier;
    public float fallJumpGravity;
    public float fastFallSpeed;

    private bool hasJumped;
    public Slider jumpChargeBar;
    public float increaseAmount = 0.5f;
    public float increasePercentage;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundDistance;
    public float groundDrag;
    public Transform groundCheck;
    [SerializeField] private bool isGrounded;
    private bool lastGrounded = true;

    [Header("SlopeHandling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    public bool exitingSlope;

    public int jumpGameCounter = 0;
    public bool isDead = false;

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        currentJumpForce = defaultJumpForce;
        jumpChargeBar.value = currentJumpForce;
        jumpChargeBar.maxValue = maxJumpForce;
        jumpChargeBar.value = 0;

        //rb.freezeRotation = true;
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //OnSlope = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded)
        {
            if (!lastGrounded)
            {
                rb.drag = groundDrag;
                exitingSlope = false;
            }
        }
        else
        {
            rb.drag = 0;
            JumpFall();
        }

        lastGrounded = isGrounded;
        ProcessInputs();
        StateHandler();

        if (currentJumpForce >= maxJumpForce)
        {
            //print("Set back to maxjumpforce");
            currentJumpForce = maxJumpForce;
        }
      
        SpeedControl();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void StateHandler()
    {
        if (wallRunning)
        {
            state = MovementStates.wallrunning;
            moveSpeed = wallrunSpeed;
        }
        if (isGrounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementStates.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (isGrounded)
        {
            state = MovementStates.walking;
            moveSpeed = walkSpeed;
        }
        else if (!wallRunning) 
        {
            state = MovementStates.air;
        }
    }
    private void ProcessInputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //if (Input.GetKey(KeyCode.Space) && isGrounded)
        //{
        //    //print("detect jump input"); 
        //    jumpChargeBar.value += increaseAmount * increasePercentage * Time.deltaTime;
        //    currentJumpForce += increaseAmount * increasePercentage * Time.deltaTime;
        //}

        //else if (Input.GetKey(KeyCode.Space) && !isGrounded) //allows for charging in air to continue chain jumps  (it sort of works)
        //{
        //    jumpChargeBar.value += increaseAmount * increasePercentage * Time.deltaTime;
        //    currentJumpForce += increaseAmount * increasePercentage * Time.deltaTime;
        //}

        if (Input.GetKey(KeyCode.Space))
        {
            jumpChargeBar.value += increaseAmount * increasePercentage * Time.deltaTime;
            currentJumpForce += increaseAmount * increasePercentage * Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space) && isGrounded)
        {
            Jump();
            jumpGameCounter++;
            currentJumpForce = defaultJumpForce;
            jumpChargeBar.value = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && state == MovementStates.air && !wallRunning)
        {
            state = MovementStates.fastfall;
            //print("checks H input");
            FastFall();
        }
    }
    private void Move()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput; //use this for jump dir

        if (OnSlope() && !exitingSlope) //when moving on slope and not exiting slope
        {
            //print("currently on slope");
            rb.AddForce(SlopeMoveDirection() * moveSpeed * 15f, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (isGrounded) //when moving and on ground
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!isGrounded) //when moving and in air
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 15f * airMultiplier, ForceMode.Force);  
        }
        rb.useGravity = !OnSlope();   
    }
    private void SpeedControl() 
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            float maxSpeed = moveSpeed;

            if (!isGrounded)
            {
                maxSpeed = maxAirVelocity; //variable maxJumpSpeed is used to change values in the inspector
            }

            if (velocity.magnitude > maxSpeed)
            {
                Vector3 limitVelocity = velocity.normalized * maxSpeed;
                rb.velocity = new Vector3(limitVelocity.x, rb.velocity.y, limitVelocity.z);
            }
        }
    }
    private void Jump()
    {
        exitingSlope = true;
        //print(exitingSlope);
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 jumpDir = orientation.forward * verticalInput + orientation.right * horizontalInput + orientation.up; //getting the player's direction
        rb.AddForce(jumpDir * currentJumpForce, ForceMode.Impulse); 
    }

    private void JumpFall()
    {
        if (rb.velocity.y <= 0.5)
        {
            rb.AddForce(Vector3.down * fallJumpGravity, ForceMode.Impulse);
        }
    }
    private void FastFall()
    {
        //print("apply downward force");

        rb.AddForce(Vector3.down * fastFallSpeed, ForceMode.Impulse);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 3f))
        {
            if (slopeHit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                //Debug.DrawRay(transform.position, Vector3.down, Color.green, 50f); //print("Hit");
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return slopeAngle < maxSlopeAngle && slopeAngle != 0;
            }
        }
        return false;
    }
    private Vector3 SlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
 
    public void Dead()
    {
        isDead = true;
    }
    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.CompareTag("DeadZone"))
    //    {
    //        print("Hit dead zone");
    //        Dead();
    //    }
    //}
}
 