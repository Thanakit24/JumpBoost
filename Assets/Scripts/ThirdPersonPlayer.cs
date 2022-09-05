using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonPlayer : MonoBehaviour
{
    public CharacterController controller;
    public Transform camera;
    public Transform groundCheck;

    public float speed = 6f;
    public float smoothTime = 0.1f;
    float smoothVelocity;

    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float gravity = -9.8f;
    public float currentJumpHeight; //test
    public float jumpHeight = 2.5f;
    bool isGrounded;
    Vector3 velocity;

    public Slider jumpChargeBar;
    public float increaseAmount = 0.5f;
    public float jumpChargeMaxValue;
    

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Move();
        Jump();
    }   

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
    }
    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            jumpChargeBar.value += increaseAmount * Time.deltaTime;
            currentJumpHeight = jumpHeight += increaseAmount * Time.deltaTime;
            Debug.Log("bar works");

        }
     
        if (Input.GetKeyUp(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(currentJumpHeight * -2f * gravity);
            jumpChargeBar.value = 0f;
            currentJumpHeight = 0f;
            jumpHeight = 2.5f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}
