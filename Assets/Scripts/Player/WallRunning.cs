using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpUpwardForce;
    public float wallJumpSideForce;
    public float wallClimbSpeed;
    //public float maxWallRunTime;
    //private float wallRunTimer;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Input")]
    private KeyCode upwardsKey = KeyCode.LeftShift;
    private KeyCode downwardsKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWall;
    private RaycastHit rightWall;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    public PlayerCamera cam;
    private FirstPersonPlayer player;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<FirstPersonPlayer>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (player.wallRunning)
        {
            WallRunningMovement();
        }
    }
    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWall, wallCheckDistance, whatIsWall);
        //Debug.DrawRay(transform.position, orientation.right, Color.green, 100f); print("hit wall");
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWall, wallCheckDistance, whatIsWall);
    }
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsKey);
        downwardsRunning = Input.GetKey(downwardsKey);

        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall) //if raycasts hits either wall and player above ground
        {
            if (!player.wallRunning)
            {
                StartWallRun();
            }

            else
            {
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    WallJump();
                }
            }

        }
        else if (exitingWall) //if leaving wall is true
        {
            exitWallTimer -= Time.deltaTime;
            
            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }
        else
        {
            if (player.wallRunning)
                player.wallRunning = false;
                WallExit();
        }
    }

    private void StartWallRun()
    {
        player.wallRunning = true;
        cam.DoFov(90f);

        if (wallLeft)
        {
            cam.DoTilt(-5f);
        }
        if (wallRight)
        {
            cam.DoTilt(5f);
        }
    }
    private void WallRunningMovement()
    {
        //print("is wall running");
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 wallNormal = wallRight ? rightWall.normal : leftWall.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }
        if (downwardsRunning)
        {
            //print("downward run wall");
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }

        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

    }
    private void WallExit()
    {
        cam.DoFov(80f);
        cam.DoTilt(0f);
    }
    private void WallJump()
    {
        exitingWall = true;
        player.jumpGameCounter++;
        exitWallTimer = exitWallTime;
        print("wall jumping");
        Vector3 wallNormal = wallRight ? rightWall.normal : leftWall.normal;
        Vector3 forceToApply = transform.up * wallJumpUpwardForce + wallNormal * player.currentJumpForce;
        //Vector3 forceToApply = transform.up * player.currentJumpForce + wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
        player.jumpChargeBar.value = 0f;
        player.currentJumpForce = 0f;
        player.wallRunning = false;

        cam.DoFov(80f);
        cam.DoTilt(0f);
    }
}
