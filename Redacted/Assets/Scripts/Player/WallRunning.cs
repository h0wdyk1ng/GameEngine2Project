using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wall Running")]
    [SerializeField] private LayerMask wall;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float wallRunForce, wallJumpUpForce, wallJumpSideForce, wallClimbSpeed, maxWallRunTime, quitWallTime;
    private float wallRunTimer, exitWallTimer;

    [Header("Inputs")]
    [SerializeField] private KeyCode runUpKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode runDownKey = KeyCode.LeftControl, jumpKey = KeyCode.Space;
    private bool runUp, runDown;
    private float hInput, vInput;

    [Header("Detection")]
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float minJumpHeight;
    private RaycastHit leftWallHit, rightWallHit;
    private bool wallLeft, wallRight, exitWall;

    [Header("Gravity")]
    [SerializeField] private bool useGravity;
    [SerializeField] private float gravityCounterForce;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private PlayerCam pCam;
    [SerializeField] private float FovRef;
    private PlayerMovement pm;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckWalls();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallRunning)
            WallRunningMvmnt();
    }

    private void CheckWalls()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, wall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, wall);
    }
    
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, ground);
    }
    
    private void StateMachine()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        runUp = Input.GetKey(runUpKey);
        runDown = Input.GetKey(runDownKey);

        if((wallLeft || wallRight) && vInput > 0 && AboveGround() && !exitWall)
        {
            if (!pm.wallRunning)
                StartWallRun();

            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0 && pm.wallRunning)
            {
                exitWall = true;
                exitWallTimer = quitWallTime;
            }

            if (Input.GetKeyDown(jumpKey)) WallJump();
        }
        else if (exitWall)
        {
            if (pm.wallRunning)
                StopWallRunning();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitWall = false;
        }
        else
        {
            if (pm.wallRunning)
                StopWallRunning();
        }
    }

    private void StartWallRun()
    {
        pm.wallRunning = true;

        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        pCam.DoFov(FovRef * 1.2f);
        if (wallLeft) pCam.DoTilt(-10);
        if (wallRight) pCam.DoTilt(10);
    }

    private void WallRunningMvmnt()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal, wallFwd = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallFwd).magnitude > (orientation.forward - -wallFwd).magnitude)
            wallFwd = -wallFwd;

        rb.AddForce(wallFwd * wallRunForce, ForceMode.Force);

        if(!(wallLeft && hInput > 0) && !(wallRight && hInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);

        if (runUp)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        if (runDown)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

        if (useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }

    private void StopWallRunning()
    {
        pm.wallRunning = false;

        pCam.DoFov(FovRef);
        pCam.DoTilt(0);
    }

    private void WallJump()
    {
        exitWall = true;
        exitWallTimer = quitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal, forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
