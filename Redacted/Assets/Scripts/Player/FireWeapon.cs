using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeapon : MonoBehaviour
{
    [Header("Fire Weapon Stats")]
    [SerializeField] private int damage;
    [SerializeField] private float timeBtwnShots, spread, range, reloadTime, timeBtwnShooting;
    [SerializeField] private int magazineSize, bulletsPerTap;
    [SerializeField] private bool holdButton;
    private int bulletsLeft, bulletsShot;

    private bool shooting, ready2Shoot, reloading;

    [Header("References")]
    [SerializeField] private Camera pCam;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private RaycastHit rHit;
    [SerializeField] private LayerMask enemyLayer;
    private Animator atr;

    [Header("Graphics")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject bulletHole, bodyHole;

    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip reloadClip, emptyClip;
    private AudioSource src;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        ready2Shoot = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        atr = GetComponent<Animator>();
        src = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        if (holdButton) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        if(ready2Shoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
            atr.Play("Shoot");
        }

        if (shooting && bulletsLeft <= 0 && emptyClip)
            StartCoroutine(PlayEmptyClip());

        if (atr)
        {
            atr.SetBool("Reload", reloading);
        }
    }

    private void Reload()
    {
        reloading = true;
        if (reloadClip) src.PlayOneShot(reloadClip);
        Invoke("FinishedReload", reloadTime);
    }
    private void Shoot()
    {
        ready2Shoot = false;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = pCam.transform.forward + new Vector3(x, y, 0);

        if (Physics.Raycast(pCam.transform.position, direction, out rHit, range, enemyLayer))
        {
            Debug.Log(rHit.collider.name);
            if (rHit.collider.CompareTag("Enemy"))
            {
                Health enemyHlth = rHit.transform.GetComponentInParent<Health>();

                if (bodyHole) Instantiate(bodyHole, rHit.point, Quaternion.LookRotation(rHit.normal));
                if (enemyHlth) enemyHlth.TakeDamage(damage);
                Debug.Log("Enemy Hit!");
            }
            else if (rHit.collider.CompareTag("Head"))
            {
                Health enemyHlth = rHit.transform.GetComponentInParent<Health>();
                //Health enemyHlth = rHit.collider.gameObject.GetComponentInParent<Health>();

                if (bodyHole) Instantiate(bodyHole, rHit.point, Quaternion.LookRotation(rHit.normal));
                if (enemyHlth) enemyHlth.TakeDamage(damage * 2);
                Debug.Log("Enemy Head Hit!");
            }
            else
            {
                if (bulletHole) Instantiate(bulletHole, rHit.point, Quaternion.LookRotation(rHit.normal));
            }

            if (rHit.rigidbody != null) rHit.rigidbody.AddForce(-rHit.normal * 10);
        }

        
        if (muzzleFlash) Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        
        if (shootClip) src.PlayOneShot(shootClip);

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBtwnShooting);
        if (bulletsShot > 0 && bulletsLeft > 0) Invoke("Shoot", timeBtwnShots);
    }

    private void ResetShot()
    {
        ready2Shoot = true;
    }

    private void FinishedReload()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    public void PlaySecondaryClip(AudioClip secClip)
    {
        src.PlayOneShot(secClip);
    }

    IEnumerator PlayEmptyClip()
    {
        ready2Shoot = false;
        src.PlayOneShot(emptyClip);
        yield return new WaitForSeconds(timeBtwnShooting);
        ready2Shoot = true;
    }
}
