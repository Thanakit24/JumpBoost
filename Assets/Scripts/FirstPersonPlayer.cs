 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonPlayer : MonoBehaviour
{
    public MovementStates state;
    public enum MovementStates 
    {
        walking,
        sprinting,
        air
    }

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    public float playerHeight = 1.5f;

    [Header("Jump")]
    public float defaultJumpForce;
    public float currentJumpForce;
    public float maxJumpForce;
    public float maxJumpSpeed;
    public float airMultiplier;
    public float fallJumpGravity;

    private bool hasJumped;
    public Slider jumpChargeBar;
    public float increaseAmount = 0.5f;
    public float increasePercentage;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundDistance;
    public float groundDrag;
    public Transform groundCheck;
    bool isGrounded;

    //[Header("SlopeHandling")]
    //public float maxSlopeAngle;
    //private RaycastHit slopeHit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentJumpForce = defaultJumpForce;
        jumpChargeBar.maxValue = maxJumpForce;
        jumpChargeBar.value = 0;

        //rb.freezeRotation = true;
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //OnSlope = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        ProcessInputs();
        StateHandler();
        SpeedControl();

        if (isGrounded)
        {
            //print(isGrounded);
            rb.drag = groundDrag;
           
        }
        else
        {
            //print("Not grounded");
            rb.drag = 0;
            JumpFall();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void StateHandler()
    {
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
        else
        {
            state = MovementStates.air;
        }
    }
    private void ProcessInputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            //print("detect jump input"); 
            // charge bar
            jumpChargeBar.value += increaseAmount * increasePercentage * Time.deltaTime;
            currentJumpForce += increaseAmount * increasePercentage * Time.deltaTime;
        }

        else if (Input.GetKey(KeyCode.Space) && !isGrounded) //allows for charging in air to continue chain jumps  (it sort of works)
        {
            jumpChargeBar.value += increaseAmount * increasePercentage * Time.deltaTime;
            currentJumpForce += increaseAmount * increasePercentage * Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space) && isGrounded)
        {
            Jump();
            currentJumpForce = defaultJumpForce;
            jumpChargeBar.value = 0;
        }
    }
    private void Move()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput; //use this for jump dir
        //if (OnSlope())
        //{
        //    rb.AddForce(SlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
        //}
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else 
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 15f * airMultiplier, ForceMode.Force);
        }
        //rb.useGravity = !OnSlope();
    }
    private void SpeedControl() 
    {
        Vector3 velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        float maxSpeed = moveSpeed;
        if (!isGrounded)
        {
            maxSpeed = maxJumpSpeed; //variable maxJumpSpeed is used to change values in the inspector
        }

        if(velocity.magnitude > maxSpeed)
        {
            Vector3 limitVelocity = velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(limitVelocity.x, rb.velocity.y, limitVelocity.z);
        }
    }
    private void Jump()
    {
        //print("function called");
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

    //private void GetDirection(Transform pressedDir)
    //{
    //    float horizontalInput = Input.GetAxisRaw("Horizontal");
    //    float verticalInput = Input.GetAxisRaw("Vertical");

    //    Vector3 direction = new Vector3();
    //    direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;

    //    if (verticalInput == 0 && horizontalInput == 0)
    //    {
    //        direction = forwardT.up;
    //        return direction.normalized;
    //    }
       
    //}
    //private bool OnSlope() 
    //{
    //    //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    //    if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f))
    //    {
    //        float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
    //        return angle < maxSlopeAngle && angle != 0;
    //    }
    //    return false;
    //}
    //private Vector3 SlopeMoveDirection()
    //{
    //    print("currently on slope");
    //    return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    //}
}
 