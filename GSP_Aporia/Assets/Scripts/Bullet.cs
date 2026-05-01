using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 100;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the bullet collides with an enemy
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            // Destroy the enemy
            print("Hit" + collision.gameObject.name + "!");

            Health ObjectHealth = collision.gameObject.GetComponent<Health>();

            if (ObjectHealth != null)
            {
                ObjectHealth.TakeDamage(damage);
            }
        }

        // Destroy the bullet after collision
        Destroy(gameObject);
    }
}
