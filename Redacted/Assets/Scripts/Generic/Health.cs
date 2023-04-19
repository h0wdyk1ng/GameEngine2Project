using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class Health : MonoBehaviour
{
    public float health = 100;
    public bool dead;
    [SerializeField] private bool IsPlayer = false;
    [SerializeField] private Shaker shake;
    [SerializeField] private ShakePreset shkPreset;
    [SerializeField] private AudioClip[] hurtClips, deadClips;

    private Animator atr;
    private AudioSource src;
    // Start is called before the first frame update
    void Start()
    {
        atr = GetComponentInChildren<Animator>();
        src = GetComponent<AudioSource>();
    }

    public void TakeDamage(float dmg)
    {
        int hurtIndex = Random.Range(0, hurtClips.Length - 1);
        health -= dmg;
        Debug.Log(this.gameObject.name + " current health: " + health);

        if (IsPlayer)
        {
            if (shake && shkPreset) shake.Shake(shkPreset);
            if (hurtClips != null) src.PlayOneShot(hurtClips[hurtIndex]);
        }

        if(health <= 0)
        {
            Dead();
        }

        if (health < 0) health = 0;
    }

    private void Dead()
    {
        int deadIndex = Random.Range(0, deadClips.Length - 1);
        dead = true;
        if (IsPlayer)
        {
            //atr.SetBool("Dead", dead);
            Debug.Log("Player died");
            if (deadClips != null) src.PlayOneShot(deadClips[deadIndex]);
            //GetComponent<PlayerMovement>().enabled = !dead;
            //GetComponent<PlayerCam>().enabled = !dead;
        }
        else
        {
            atr.enabled = false;
            Debug.Log(this.gameObject.name + " is dead...");
            if (deadClips != null) src.PlayOneShot(deadClips[deadIndex]);
            GetComponent<EnemyAiVer2>().enabled = false;
            GetComponent<SoundPlayer>().enabled = false;
            Destroy(this.gameObject, 11);
        }
    }
}
