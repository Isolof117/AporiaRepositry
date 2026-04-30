using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{

    [Header("Patrol variables")]

    [SerializeField] private Transform[] patrolNodes;

    int nodePointer = 0;

    [SerializeField] private float walkSpeed = 5.0f;


    [Header("Player References")]

    [SerializeField] private Transform playerRef;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;

   

    [Header("Sight Parameters")]

    [SerializeField] private float sightDistance;
    [SerializeField] private float fovRange = 80.0f;


    [Header("Other")]

    [SerializeField] private bool isIdle = false;

    private NavMeshAgent agent;


    enum EnemyState 
    { 
        Patrol,
        Search,
        Attack
    }

    [SerializeField] private EnemyState enemyState;
    private void Start()
    {
        enemyState = EnemyState.Patrol;
        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(patrolNodes[nodePointer].position);
        agent.speed = walkSpeed;
      
    }


    // Update is called once per frame
    void Update()
    {
        if(!isIdle)
        {
            //CHECK FOR STATE CHANGE

            CheckForStateChange();


            //Patrol Mode

            if (enemyState == EnemyState.Patrol)
            {
                agent.stoppingDistance = 0f;
                Patrol();

            }
            //Attack Mode

            if (enemyState == EnemyState.Attack)
            {
                agent.stoppingDistance = 2f;
                //Step closer to target

                agent.SetDestination(playerRef.position);

            }

        }


    }

    void CheckForStateChange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerRef.position);

        bool playerVisible = false;
        bool inFOV = false;

        RaycastHit hit;

        //Check if player is in eye range

        if (distanceToPlayer < sightDistance)
        {
            //Check if player is in FOV (peripherals)
            inFOV = CheckInFOV();

            Vector3 towardsPlayer = (playerRef.position - transform.position).normalized;

            LayerMask visionMask = playerLayer | wallLayer;

            //Check if player is not behind any walls

            if (Physics.Raycast(transform.position, towardsPlayer, out hit, distanceToPlayer, visionMask))
            {
                Debug.DrawRay(transform.position, towardsPlayer * distanceToPlayer, Color.green);

                Debug.Log("Hit:" + hit.collider.name);

                if ((playerLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    playerVisible = true;
                }
            }
        }

        if (inFOV && playerVisible)
        {
            enemyState = EnemyState.Attack;
            
        }
        else
        {
            enemyState = EnemyState.Patrol;

            agent.SetDestination(patrolNodes[nodePointer].position);
          
        }
    }
    bool CheckInFOV()
    {
        Vector3 towardsPlayerVector = (playerRef.position - transform.position).normalized;

        Vector3 leftVector = Quaternion.AngleAxis(-fovRange, Vector3.up) * transform.forward;

        Vector3 rightVector = Quaternion.AngleAxis(fovRange, Vector3.up) * transform.forward;

        bool isInSightRange = IsBetweenVectors(leftVector, rightVector, towardsPlayerVector);

        return isInSightRange;
    }
    bool IsBetweenVectors(Vector3 left, Vector3 right, Vector3 t)
    {
        left.Normalize();
        right.Normalize();
        t.Normalize();

        float dotLR = Vector3.Dot(left, right);
        float dotLT = Vector3.Dot(left, t);
        float dotRT = Vector3.Dot(right, t);

        return dotLT >= dotLR && dotRT >= dotLR;

    }

    void Patrol()
    {   

        //Iterate to next node if enemy has made it to target position
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            if (nodePointer == patrolNodes.Length - 1)
            {
                nodePointer = 0;
            }
            else
            {
                nodePointer++;
            }

            agent.SetDestination(patrolNodes[nodePointer].position);
        }
        
    }


    private void OnDrawGizmos()
    {
        //Enemy range sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);

        //Enemy fov gizmos
        Gizmos.color = Color.red;

        Vector3 leftVector = Quaternion.AngleAxis(-fovRange, Vector3.up) * transform.forward;

        Vector3 rightVector = Quaternion.AngleAxis(fovRange, Vector3.up) * transform.forward;

        Vector3 forwardPoint = transform.position + (transform.forward * sightDistance);
        Vector3 leftPoint = transform.position + (leftVector * sightDistance);
        Vector3 rightPoint = transform.position + (rightVector * sightDistance);

        Gizmos.DrawLine(transform.position, forwardPoint);
        Gizmos.DrawLine(transform.position, leftPoint);
        Gizmos.DrawLine(transform.position, rightPoint);

        //Enemy path gizmos

        for(int i = 0; i < patrolNodes.Length; i++)
        {
            //Circles for nodes
            Gizmos.color = Color.magenta;

            Gizmos.DrawSphere(patrolNodes[i].position, 0.3f);

            //Draw lines to connect nodes
            Gizmos.color = Color.cyan;

            if (i != patrolNodes.Length - 1)
            {
                
                Gizmos.DrawLine(patrolNodes[i].position, patrolNodes[i + 1].position);
            }
            else
            {
                Gizmos.DrawLine(patrolNodes[i].position, patrolNodes[0].position);
            }
        }


    }
}
