using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask ground, playerLayer;

    [SerializeField] private float timeBetweenAttacks, atkRange;
    [SerializeField] private int nmbrOfAltAttacks = 1; // max attacks - 1
    bool alreadyAttacked, playerInAttackRange;
    [SerializeField] private bool isRanged = false;
    [SerializeField] private GameObject projectile, testHitbox;
    [SerializeField] private Animator atr;

    // Start is called before the first frame update
    void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
        testHitbox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, atkRange, playerLayer);

        if (!playerInAttackRange)
            ChasePlayer();
        else
            AttackPlayer();
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        if (atr)
        {
            atr.SetFloat("Speed", 1, .3f, Time.deltaTime);
            atr.SetBool("Attack", false);
        }
    }

    private void AttackPlayer()
    {

        agent.SetDestination(transform.position);
        if (atr)
        {
            atr.SetFloat("Speed", 0, .3f, Time.deltaTime);
            atr.SetBool("Attack", true);
        }
        
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // Attack code here
            if (isRanged)
            {
                Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                
                rb.AddForce(transform.forward * 48, ForceMode.Impulse);
            }
            else
            {
                StartCoroutine(TestAttack());
            }
            ///
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = true;
    }

    public void EnableHitbox(GameObject hitbox)
    {
        hitbox.SetActive(true);
    }

    public void DisableHitbox(GameObject hitbox)
    {
        hitbox.SetActive(false);
    }

    IEnumerator TestAttack()
    {
        yield return new WaitForSeconds(.1f);
        testHitbox.SetActive(true);
        yield return new WaitForSeconds(.2f);
        testHitbox.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRange);
    }
}
