using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class SimpleBulletScript : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float BulletSpeed;
    [SerializeField] private int timeBeforeDrop;
    Vector3 bulletForce;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.position = transform.position;
        bulletForce = new Vector3 ( 0, 0, BulletSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(bulletForce, ForceMode.Impulse);
    }


    // 
    void bulletBehaviour(float _bulletSpeed, int _timeBeforeDrop)
    {
        


    }
}
