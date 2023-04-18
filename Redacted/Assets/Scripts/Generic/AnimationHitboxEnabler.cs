using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHitboxEnabler : MonoBehaviour
{
    [SerializeField] private GameObject hitbox;
    
    private void Start()
    {
        hitbox.SetActive(false);
    }

    public void EnableHitbox()
    {
        hitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        hitbox.SetActive(false);
    }
}
