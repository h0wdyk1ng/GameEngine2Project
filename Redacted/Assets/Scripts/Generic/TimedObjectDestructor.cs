using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectDestructor : MonoBehaviour
{
    [SerializeField] private float wait;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, wait);
    }
}
