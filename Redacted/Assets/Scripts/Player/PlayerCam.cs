using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private float sensX, sensY;
    [SerializeField] private Transform orientation, camHolder;

    float yRot, xRot;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

        yRot += mX;
        xRot -= mY;
        xRot = Mathf.Clamp(xRot, -90, 90);

        camHolder.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}
