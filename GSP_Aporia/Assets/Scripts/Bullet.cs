using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the bullet collides with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Destroy the enemy
            print("Hit" + collision.gameObject.name + "!");
            Destroy(collision.gameObject);
        }
        // Destroy the bullet after collision
        Destroy(gameObject);
    }
}
