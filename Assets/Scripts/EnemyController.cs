using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;

    Transform player;

    [SerializeField] float attackRange = 3f;
    [SerializeField] float chaseRange = 9f;
    [SerializeField] float turnSpeed = 15f;
    [SerializeField] float patrolRadius = 8f;
    [SerializeField] float patrolWait = 2f;

    [SerializeField] float chaseSpeed = 5f;
    [SerializeField] float searchSpeed = 3f;

    private bool isSearched;

    enum State
    {
        Idle,
        Search,
        Chase,
        Attack
    } // Here we created an enum.

    [SerializeField] State currentState = State.Idle;  // Here we decleare an enum variable as currentState and we set it State.Idle 
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        StateCheck();
        StateExecute();

    }

    private void StateCheck()
    {
        // Here we set enemy's state according to distance to player.

        float distanceToTarget = Vector3.Distance(player.position, transform.position);
        if (distanceToTarget < chaseRange && distanceToTarget > attackRange)
        {
            currentState = State.Chase;
        }
        else if (distanceToTarget <= attackRange)
        {
            currentState = State.Attack;
        }
        else
        {
            currentState = State.Search;
        }
    }

    void StateExecute()
    {
        // Here we can make enemy events for current state status.

        switch (currentState)
        {
            case State.Idle:
                break;
            case State.Search:
                if (!isSearched && agent.remainingDistance <= 0.1f  ||  !agent.hasPath && !isSearched)
                {
                    Vector3 agentTarget = new Vector3(agent.destination.x, transform.position.y, agent.destination.z);
                    agent.enabled = false;
                    transform.position = agentTarget;
                    agent.enabled = true;
                    Invoke("Search", patrolWait);
                    isSearched = true;
                    
                }
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;

        }
    }

    void Attack()
    {
        if (player == null)
        {
            return;
        }
        agent.isStopped = true;
        LookToTarget(player.position);
    }

    void Chase()
    {
        if (player == null)
        {
            return;
        }
        agent.isStopped = false;
        agent.SetDestination(player.position);
        agent.speed = chaseSpeed;
    }

    void Search()
    {
        agent.isStopped = false;
        isSearched = false;
        agent.SetDestination(GetRandomPosition());
        agent.speed = searchSpeed;
    }
    void LookToTarget(Vector3 target)
    {
        Vector3 lookPos = new Vector3(target.x, transform.position.y, target.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos - transform.position), turnSpeed*Time.deltaTime);
    }

    Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * patrolRadius;
        if (Vector3.Distance(transform.position,new Vector3(randomDirection.x,transform.position.y,randomDirection.z)) <= (patrolRadius/2.5f) )
        {
            randomDirection *= 2;
        }
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius,NavMesh.AllAreas);
        return hit.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
        switch (currentState)
        {
            case State.Search:
                Gizmos.color = Color.blue;
                Vector3 destinationPos = new Vector3(agent.destination.x, transform.position.y, agent.destination.z);
                Gizmos.DrawLine(transform.position, destinationPos);
                break;
            case State.Chase:
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, player.position);
                break;
            case State.Attack:
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, player.position);
                break;

        }
    }
}


