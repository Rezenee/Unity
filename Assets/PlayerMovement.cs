using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -30;
    public float jumpHeight = 3f;
    public Vector3 x_vel = (0,0,0);
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        Vector3 move = transform.right * x + transform.forward * z;
        x_vel = move;

        controller.Move(x_vel * speed * Time.deltaTime);
        /*
        print(transform.right + " transform.right");
        print(x + " x");
        print(transform.forward + " transform.forward");
        print(z + " z");
        */
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += (gravity * Time.deltaTime);

        controller.Move(velocity * Time.deltaTime);

    }
}
