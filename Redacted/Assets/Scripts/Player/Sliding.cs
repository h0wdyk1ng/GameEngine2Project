using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce, slideYScale;
    private float slideTimer, startYScale;

    [Header("Input")]
    [SerializeField] private KeyCode slideKey = KeyCode.LeftControl;
    private float xInput, yInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(slideKey) && (xInput != 0 || yInput != 0))
            StartSlide();

        if (Input.GetKeyUp(slideKey) && pm.sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        pm.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDir = orientation.forward * yInput + orientation.right * xInput;

        if(!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDir.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(pm.GetSlopeMoveDir(inputDir) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        pm.sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
