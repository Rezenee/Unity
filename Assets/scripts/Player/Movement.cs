 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Strafe Variables
    public float gravity = 3f;
    public float ground_accelerate = 50f;
    public float max_velocity_ground = 4f;
    public float air_accelerate = 150f;
    public float max_velocity_air = 2f;
    public float friction = 8;
    bool onGround;
    public float jump_force = 5f;
    private Vector3 lastFrameVelocity = Vector3.zero;
    Vector3 originalPos;
    public Camera camObj;

    Rigidbody rb;
    Collider coll;

    void Start()
    {
        originalPos = gameObject.transform.position;
        rb = GetComponent<Rigidbody>(); 
        coll = GetComponent<Collider>();
    }


    void Update()
    {
        Vector2 input;
        bool ground = Grounded();
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 tempVelocity = CalculateFriction(rb.velocity);

        if (ground)
        {
            tempVelocity += CalculateMovementGround(input, tempVelocity);
        }
        else
        {
            tempVelocity += CalculateMovementAir(input, tempVelocity);
        }
        rb.velocity = tempVelocity;
        lastFrameVelocity = rb.velocity;

        rb.velocity += new Vector3(0f, -gravity, 0f) * Time.deltaTime;
    }

    public Vector3 CalculateFriction(Vector3 currentVelocity)
    {
        onGround = Grounded();
        float speed = currentVelocity.magnitude;

        //Code from https://flafla2.github.io/2015/02/14/bunnyhop.html
        if (!onGround || Input.GetButton("Jump") || speed == 0f)
            return currentVelocity;

        float drop = speed * friction * Time.deltaTime;
        return currentVelocity * (Mathf.Max(speed - drop, 0f) / speed);
    }

    //Do movement input here
    public Vector3 CalculateMovementGround(Vector2 input, Vector3 velocity)
    {
        //Get rotation input and make it a vector
        // Format of camRotation is (0.0, 0-360, 0.0) changes depending on where you are looking.
        Vector3 camRotation = new Vector3(0f, camObj.transform.rotation.eulerAngles.y, 0f);
        // Input velocity is (-100-100, 0, -100-100) It changes depending on where you are looking
        // +100 is straight forwards, -100 is straight backwards. 
        Vector3 inputVelocity = Quaternion.Euler(camRotation) *
            new Vector3(input.x * ground_accelerate, 0f, input.y * ground_accelerate);
        //Ignore vertical component of rotated input
        Vector3 alignedInputVelocity = new Vector3(inputVelocity.x, 0f, inputVelocity.z) * Time.deltaTime;
        //alignedInputVelocity is inputvelocity, but scaled so it doesn't change depending on your framerate.
        //Get current velocity
         
        Vector3 currentVelocity = new Vector3(velocity.x, 0f, velocity.z);
        //currentVelocity is (0.0, 0.0, 0.0) that scales depending on how fast you are moving.

        alignedInputVelocity += GetJumpVelocity(velocity.y);
        return alignedInputVelocity;
    }

        public Vector3 CalculateMovementAir(Vector2 input, Vector3 velocity)
    {
        onGround = Grounded();
        Vector3 camRotation = new Vector3(0f, camObj.transform.rotation.eulerAngles.y, 0f);

        Vector3 inputVelocity = Quaternion.Euler(camRotation) *
            new Vector3(input.x * air_accelerate, 0f, input.y * air_accelerate);
        print(Quaternion.Euler(camRotation));

        Vector3 alignedInputVelocity = new Vector3(inputVelocity.x, 0f, inputVelocity.z) * Time.deltaTime;
  

        Vector3 currentVelocity = new Vector3(velocity.x, 0f, velocity.z);
        float max = Mathf.Max(0f, 1 - (currentVelocity.magnitude / max_velocity_air));
        //How perpendicular the input to the current velocity is (0 = 90°)
        float velocityDot = Vector3.Dot(currentVelocity, alignedInputVelocity);
        //Scale the input to the max speed
        Vector3 modifiedVelocity = alignedInputVelocity * max;

        //The more perpendicular the input is, the more the input velocity will be applied
        Vector3 correctVelocity = Vector3.Lerp(alignedInputVelocity, modifiedVelocity, velocityDot);
        //Apply jump
        correctVelocity += GetJumpVelocity(velocity.y);
        //Return
        return correctVelocity;
    }

    private Vector3 GetJumpVelocity(float yVelocity)
    {
        Vector3 jumpVelocity = Vector3.zero;

        //Calculate jump
        if (Input.GetButton("Jump") && yVelocity < jump_force && Grounded())
        {
            jumpVelocity = new Vector3(0f, jump_force - yVelocity, 0f);
        }

        return jumpVelocity;
    }


    bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, coll.bounds.extents.y + 0.1f);
    }


}