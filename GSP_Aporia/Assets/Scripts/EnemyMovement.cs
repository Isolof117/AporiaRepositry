using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{

    [Header("Patrol variables")]

    [SerializeField] private Transform[] patrolNodes;

    int nodePointer = 0;


    [Header("Player References")]

    [SerializeField] private Transform playerRef;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;



    [Header("Sight Parameters")]

    [SerializeField] private float sightDistance;
    [SerializeField] private float fovRange = 80.0f;

    [Header("Death Variables")]

    [SerializeField] private GameObject deathPickUp;

    [Header("Other")]

    [SerializeField] private bool isIdle = false;
    [SerializeField] public WeaponBase Weapon;
    //[SerializeField] private WeaponData data;

    private NavMeshAgent agent;

    public Health healthScript;

    public enum EnemyType
    {
        Heavy,
        Medium,
        Burst,
        Light
    }

    enum EnemyState
    {
        Patrol,
        Search,
        Attack
    }

    [SerializeField] private EnemyState enemyState;
    [SerializeField] private EnemyType enemyType;
    private bool enemyIsFiring;

    private void Start()
    {
        enemyState = EnemyState.Patrol;
        DetermineEnemyType();

        Weapon.bulletsLeft = Weapon.magazineSize;
        Weapon.ResetProperties();

        agent = GetComponent<NavMeshAgent>();

        Weapon.isAiControlled = true;
        Weapon.target = playerRef;

        agent.SetDestination(patrolNodes[nodePointer].position);

    }

    private void OnEnable()
    {
        //Subscribe to events

        healthScript.OnDeath += HandleEnemyDeath;
    }




    // Update is called once per frame
    void Update()
    {
        if (!isIdle)
        {
            //CHECK FOR STATE CHANGE

            CheckForStateChange();


            //Patrol Mode

            if (enemyState == EnemyState.Patrol)
            {

                Patrol();

            }
            //Attack Mode

            if (enemyState == EnemyState.Attack)
            {

                //Step closer to target

                agent.SetDestination(playerRef.position);

                if (Weapon.bulletsLeft <= 0 && !Weapon.isReloading)
                {
                    Weapon.Reload();
                    return;
                }

                if (Weapon != null)
                    Weapon.Fire();
            }

        }


    }

    void CheckForStateChange()
    {
        EnemyState previousState = enemyState;

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

        }

        //Check if state has changed

        if (previousState != enemyState)
        {
            OnStateChange();
        }


    }

    void OnStateChange()
    {
        //Modify navmesh agent values when switching states

        switch (enemyState)
        {
            case EnemyState.Patrol:

                agent.SetDestination(patrolNodes[nodePointer].position);
                agent.stoppingDistance = 0f;

                break;

            case EnemyState.Attack:

                agent.SetDestination(playerRef.position);
                agent.stoppingDistance = 5.0f;

                break;

            default:

                Debug.LogError("Enemy State '" + enemyState + "' does not exist! ");
                break;
        }

    }

    void DetermineEnemyType()
    {
        switch (enemyType)
        {
            case EnemyType.Heavy:
                {
                    Weapon.currentMode = WeaponBase.ShootingMode.Auto;
                    Weapon.magazineSize = 30;
                    Weapon.fireRate = 0.3f;
                    break;
                }
            case EnemyType.Medium:
                {
                    Weapon.currentMode = WeaponBase.ShootingMode.Auto;
                    Weapon.magazineSize = 15;
                    Weapon.fireRate = 0.73f;
                    break;
                }
            case EnemyType.Burst:
                {
                    Weapon.currentMode = WeaponBase.ShootingMode.Burst;
                    Weapon.magazineSize = 20;
                    Weapon.fireRate = 0.5f;
                    Weapon.bulletsPerBurst = 3;
                    break;
                }
            case EnemyType.Light:
                {
                    Weapon.currentMode = WeaponBase.ShootingMode.Single;
                    Weapon.magazineSize = 10;
                    Weapon.fireRate = 0.8f;
                    break;
                }
            default:
                {
                    Debug.LogError("Enemy Type '" + enemyType + "' does not exist! ");
                    break;
                }
        }
    }

    bool CheckInFOV() //Check if the player is in the angle inbetween the enemies view
    {
        Vector3 towardsPlayerVector = (playerRef.position - transform.position).normalized;

        float angle = Vector3.Angle(transform.forward, towardsPlayerVector);

        return angle <= (fovRange * 0.5f);

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


    void HandleEnemyDeath()
    {
        Debug.Log("Enemy Death event called");
        GameObject deathPickUpInstance = Instantiate(deathPickUp, transform.position, transform.rotation);
        WeaponData data = deathPickUpInstance.GetComponent<WeaponData>();

        if (data != null)
            data.GetData(Weapon);

        Die();
    }

    IEnumerator EnemyFire()
    {
        Weapon.Fire();
        yield return new WaitForSeconds(Weapon.fireRate);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        //Enemy range sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);

        //Enemy fov gizmos
        Gizmos.color = Color.red;

        float halfFOV = fovRange * 0.5f;

        Vector3 leftFOV = Quaternion.AngleAxis(-halfFOV, Vector3.up) * transform.forward;
        Vector3 rightFOV = Quaternion.AngleAxis(halfFOV, Vector3.up) * transform.forward;

        Vector3 leftPoint = transform.position + leftFOV * sightDistance;
        Vector3 rightPoint = transform.position + rightFOV * sightDistance;

        Gizmos.DrawLine(transform.position, leftPoint);
        Gizmos.DrawLine(transform.position, rightPoint);

    }

    private void OnDrawGizmos()
    {
        //Enemy path gizmos

        for (int i = 0; i < patrolNodes.Length; i++)
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

    private void OnDisable()
    {
        //Unsubscribe to events

        healthScript.OnDeath -= HandleEnemyDeath;
    }
}
