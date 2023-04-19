using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    [SerializeField] private int selWpn = 0;
    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelWpn = selWpn;

        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selWpn >= transform.childCount - 1)
                selWpn = 0;
            else
                selWpn++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selWpn <= 0)
                selWpn = transform.childCount - 1;
            else
                selWpn--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selWpn = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && transform.childCount >= 2)
        {
            selWpn = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && transform.childCount >= 3)
        {
            selWpn = 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && transform.childCount >= 4)
        {
            selWpn = 3;
        }

        if (previousSelWpn != selWpn)
        {
            SelectWeapon();
        }
    }

    private void SelectWeapon()
    {
        int i = 0;
        foreach(Transform wpn in transform)
        {
            if (i == selWpn) wpn.gameObject.SetActive(true);
            else wpn.gameObject.SetActive(false);

            i++;
        }
    }
}
