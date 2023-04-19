using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowbarAttack : MonoBehaviour
{
    [SerializeField] private GameObject hitbox;

    private Animator atr;
    private bool attack;

    // Start is called before the first frame update
    void Start()
    {
        atr = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !attack) StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        int i = Random.Range(1, 4);
        attack = true;
        atr.Play("crowbar_attack" + i);
        yield return new WaitForSeconds(0.16667f);
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.16667f);
        hitbox.SetActive(false);
        yield return new WaitForSeconds(0.4f);
        attack = false;
    }
}
