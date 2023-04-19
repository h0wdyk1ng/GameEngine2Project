using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private string tagToHurt = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(tagToHurt) && other.gameObject.GetComponentInParent<Health>())
        {
            other.gameObject.GetComponentInParent<Health>().TakeDamage(damage);
        }
    }
}
