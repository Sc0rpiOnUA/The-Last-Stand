using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform bulletDirection, bulletCollector, ground;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public AudioSource source;
    public AudioClip clip;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        try
        {
            player = GameObject.Find("Player").transform;
            bulletCollector = GameObject.FindWithTag("BulletCollector").transform;
            ground = GameObject.FindWithTag("Ground").transform;
            bulletDirection = transform.Find("BulletSpawner");

            agent = GetComponent<NavMeshAgent>();
        }
        catch (MissingReferenceException ex)
        {
            Debug.Log(ex);
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            bool isPlayerBullet = other.gameObject.GetComponent<Bullet>().isPlayerBullet;

            if (isPlayerBullet)
                HitByBullet();
        }
    }

    private void Update()
    {        
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInSightRange && playerInAttackRange) AttackPlayer();
        else if (playerInSightRange) ChasePlayer();
        else Patroling();
    }

    public void HitByBullet()
    {
        player.GetComponent<PlayerController>().AddScore();        
        Destroy(gameObject);
    }

    private void SearchWalkPoint()
    {
        Vector3 groundSize = new Vector3();
        try
        {
            groundSize = ground.GetComponent<Renderer>().bounds.size;
        }
        catch (MissingReferenceException ex)
        {
            Debug.Log(ex);
            return;
        }

        walkPoint = new Vector3(Random.Range(groundSize.x / 2, -groundSize.x / 2), 1, Random.Range(groundSize.z / 2, -groundSize.z / 2));

        if (Physics.Raycast(walkPoint, -transform.up, 5f, whatIsGround)) walkPointSet = true;
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);
       
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(player.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if(!alreadyAttacked)
        {
            source.PlayOneShot(clip);
            GameObject g = Instantiate(bullet, bulletDirection.position, bulletDirection.rotation, bulletCollector);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, attackRange);
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, sightRange);
    //}

}
