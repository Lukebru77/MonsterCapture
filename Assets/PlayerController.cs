using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public float gravity;

    private bool isGrounded;
    private bool hasCoyoted = false;
    private float lastGroundedTime = float.NegativeInfinity;
    private float jumpInputTime = float.NegativeInfinity;

    public Rigidbody rb;
    public LayerMask groundMask;

    Vector3 dampVelocity;
    Vector2 airDampVelocity;

    public float airControlMultiplier = 1.6f;
    public float maxSpeed = 10f;

    [SerializeField] private Camera camera;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (camera == null)
        {
            camera = Camera.main ? Camera.main : FindObjectOfType<Camera>();
        }
    }

    private void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        Movement();

    }



    private void Movement()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Vector3 inputTransformed = camera.transform.TransformDirection(input);
        inputTransformed.y = 0f;
        input = inputTransformed.normalized * input.magnitude;

        if (input.magnitude > 1)
        {
            input.Normalize();
        }
        input *= speed * Time.deltaTime;

        if (isGrounded)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector3(input.x, rb.velocity.y, input.z),
                                                    ref dampVelocity, 0.1f);
            airDampVelocity = Vector2.zero;
        }
        else
        {
            dampVelocity = Vector3.zero;
            rb.AddForce(new Vector3(input.x, 0f, input.z) * airControlMultiplier, ForceMode.Acceleration);
            Vector2 xzMovement = new Vector2(rb.velocity.x, rb.velocity.z);
            if (rb.velocity.magnitude > maxSpeed)
            {
                xzMovement = Vector2.SmoothDamp(xzMovement, xzMovement.normalized * maxSpeed,
                                                ref airDampVelocity, 0.1f);

                rb.velocity = new Vector3(xzMovement.x, rb.velocity.y, xzMovement.y);
            }

        }


    }

    //private bool hasCoyoted = false;
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInputTime = Time.time;
        }

        if (isGrounded || !hasCoyoted && (Time.time - lastGroundedTime) < 0.5f)
        {
            if ((Time.time - jumpInputTime) < 0.5f)
            {
                hasCoyoted = true;
                lastGroundedTime = float.NegativeInfinity;
                jumpInputTime = float.NegativeInfinity;
                rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        hasCoyoted = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        int goLayer = 1 << collision.gameObject.layer;

        if ((groundMask & goLayer) != 0)
        {
            //Debug.Log("Grounded");

            isGrounded = true;
            lastGroundedTime = Time.time;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        int goLayer = 1 << collision.gameObject.layer;

        if ((groundMask & goLayer) != 0)
        {
            isGrounded = false;
        }
    }
}