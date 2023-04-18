using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiVer2 : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject testHitbox;
    [SerializeField] private AudioClip[] atkClips;
    private NavMeshAgent agent;
    private Animator atr;
    private AudioSource src;
    private Health health;
    private bool attack = false;
    private float lastAttackTime;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        atr = GetComponentInChildren<Animator>();
        src = GetComponent<AudioSource>();
        health = GetComponent<Health>();
        if (testHitbox) testHitbox.SetActive(false);
        if (target == null) target = GameObject.Find("PlayerHolder").transform;
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsTarget();
        if (atr) atr.SetBool("Attack", attack);
        if (attack)
            StartCoroutine(TestAttack());
    }

    void MoveTowardsTarget()
    {
        agent.SetDestination(target.position + Vector3.up);
        if (atr) atr.SetFloat("Speed", 1, 0.3f, Time.deltaTime);
        RotateToTarget();

        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (distanceToTarget <= agent.stoppingDistance && !attack)
        {
            if (atr)
            {
                atr.SetFloat("Speed", 0, 0.3f, Time.deltaTime);
            }
            if (Time.time >= lastAttackTime + 2)
            {
                lastAttackTime = Time.time;
                attack = true;
            }
        }
    }
    
    void RotateToTarget()
    {
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up * 2);
        transform.rotation = rotation;
    }

    IEnumerator TestAttack()
    {
        int atkIndex = Random.Range(0, atkClips.Length - 1);
        if (testHitbox)
        {
            yield return new WaitForSeconds(1);
            testHitbox.SetActive(true);
            yield return new WaitForSeconds(.1f);
            testHitbox.SetActive(false);
            yield return new WaitForSeconds(2);
            attack = false;
        }
        src.PlayOneShot(atkClips[atkIndex]);
        yield return new WaitForSeconds(2);
        attack = false;
    }

    public bool IsDead()
    {
        return health.dead;
    }
}
