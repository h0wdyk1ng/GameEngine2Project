using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Aiming : MonoBehaviour
{
    [SerializeField] private Transform aimPos;
    private Vector3 origPos;
    private bool aiming = false;
    // Start is called before the first frame update
    void Start()
    {
        origPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        aiming = Input.GetKey(KeyCode.Mouse1);

        if (aiming)
        {
            transform.DOLocalMove(aimPos.localPosition, 0.25f);
        }
        else
        {
            transform.DOLocalMove(origPos, 0.25f);
        }
    }
}
