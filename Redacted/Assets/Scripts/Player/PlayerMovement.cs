using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed, sprintSpeed, groundDrag, jumpForce, jumpCooldown, airMultiplier, crouchSpeed, crouchYScale, slideSpeed, wallRunSpeed;
    private float startYScale, desMoveSpeed, lastDesMoveSpeed;
    private bool readyToJump;
    public bool sliding, wallRunning;
    public float speedIncMult, slopeIncMult;

    [SerializeField] private KeyCode jumpKey = KeyCode.Space, sprintKey = KeyCode.LeftShift, crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    private bool grounded;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [SerializeField] private Transform orientation;

    [SerializeField] private enum MovementState
    {
        walk,
        sprint,
        wallRunning,
        crouching,
        sliding,
        air
    }

    [SerializeField] private MovementState state;

    float hInput, vInput;

    Vector3 moveDir;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startYScale = transform.localScale.y;
        ResetJump();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);

        MyInput();

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        SpeedControl();
        StateHandler();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
    }

    private void MovePlayer()
    {
        moveDir = orientation.forward * vInput + orientation.right * hInput;

        if(grounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10 * airMultiplier, ForceMode.Force);

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDir(moveDir) * moveSpeed * 20, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80, ForceMode.Force);
            }
        }

        if(!wallRunning) rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limVel.x, rb.velocity.y, limVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private void StateHandler()
    {
        if (wallRunning)
        {
            state = MovementState.wallRunning;
            desMoveSpeed = wallRunSpeed;
        }
        else if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desMoveSpeed = slideSpeed;
            else
                desMoveSpeed = sprintSpeed;
        }
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desMoveSpeed = crouchSpeed;
        }
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprint;
            desMoveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walk;
            desMoveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
            desMoveSpeed = (walkSpeed + sprintSpeed) / 2;
        }

        if(Mathf.Abs(desMoveSpeed - lastDesMoveSpeed) > 6 && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desMoveSpeed;
        }

        lastDesMoveSpeed = desMoveSpeed;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDir(Vector3 dir)
    {
        return Vector3.ProjectOnPlane(dir, slopeHit.normal).normalized;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0, difference = Mathf.Abs(desMoveSpeed - moveSpeed), startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleInc = 1 + (slopeAngle / 90);
                time += Time.deltaTime * speedIncMult * slopeIncMult * slopeAngleInc;
            }
            else
                time += Time.deltaTime * speedIncMult;

            yield return null;
        }

        moveSpeed = desMoveSpeed;
    }
}
