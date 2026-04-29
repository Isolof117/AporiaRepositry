using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform[] patrolNodes;

    int nodePointer = 0;

    [SerializeField] private Transform playerRef;

    [SerializeField] private float patrolSpeed;

    [SerializeField] private float sightDistance;
    [SerializeField] private float fovRange = 80.0f;

    [SerializeField] private LayerMask playerLayer;


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
    }


    // Update is called once per frame
    void Update()
    {
        if(enemyState == EnemyState.Patrol)
        {
            Patrol();

        }

        if (enemyState == EnemyState.Attack)
        {
            float step = patrolSpeed * Time.deltaTime;

            //Step closer to target

            transform.position = Vector3.MoveTowards(transform.position, playerRef.position, step);

            transform.LookAt(playerRef.position);

        }

        //CHECK FOR STATE CHANGE
        Vector3 towardsPlayerVector = playerRef.position - transform.position;

        Vector3 leftVector = Quaternion.AngleAxis(-fovRange, Vector3.up) * transform.forward;

        Vector3 rightVector = Quaternion.AngleAxis(fovRange, Vector3.up) * transform.forward;
        
        bool isInSightRange = IsBetweenVectors(leftVector, rightVector, towardsPlayerVector);

        Collider[] hits = Physics.OverlapSphere(transform.position, sightDistance, playerLayer);


        if (isInSightRange && hits.Length > 0)
        {
            enemyState = EnemyState.Attack;
        }
        else
        {
            enemyState = EnemyState.Patrol;
            transform.LookAt(new Vector3(patrolNodes[nodePointer].position.x, transform.position.y, patrolNodes[nodePointer].position.z), Vector3.up);
        }


    }
    bool IsBetweenVectors(Vector3 left, Vector3 right, Vector3 t)
    {
        left.Normalize();
        right.Normalize();
        t.Normalize();

        float dotLR = Vector3.Dot(left, right);
        float dotLT = Vector3.Dot(left, t);
        float dotRT = Vector3.Dot(left, t);

        return dotLT >= dotLR && dotRT >= dotLR;

    }

    void Patrol()
    {
        Vector3 targetPosition = patrolNodes[nodePointer].position;
        float step = patrolSpeed * Time.deltaTime;

        //Step closer to target

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        //Iterate to next node if enemy has made it to target position
        if (Vector3.Distance(transform.position, targetPosition) <= 1)
        {
            if (nodePointer == patrolNodes.Length - 1)
            {
                nodePointer = 0;
            }
            else
            {
                nodePointer++;
            }

            transform.LookAt(new Vector3(patrolNodes[nodePointer].position.x, transform.position.y, patrolNodes[nodePointer].position.z),Vector3.up);

        }
    }


    private void OnDrawGizmos()
    {
        //Enemy range sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);

        //Vector to player line
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, playerRef.position);


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
