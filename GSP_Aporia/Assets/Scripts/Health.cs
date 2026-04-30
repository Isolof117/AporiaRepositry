using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private GameObject Self;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;


    private void Start()
    {
        Self = gameObject.GetComponent<GameObject>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            //Object is dead (call OnDeath method here)
            Destroy(Self);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision != null && CompareTag("LightBullet"))
        {
            TakeDamage(5);
        }

        if (collision != null && CompareTag("MediumBullet"))
        {
            TakeDamage(10);
        }

        if (collision != null && CompareTag("HeavyBullet"))
        {
            TakeDamage(20);
        }
    }
}
