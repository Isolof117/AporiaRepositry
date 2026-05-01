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
<<<<<<< Updated upstream
=======
    private void Update()
    {
        healthSlider.value = currentHealth;
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            //Test Damage on Enemy
            TakeDamage(10);
        }
    }

    private void LateUpdate()
    {
        canvas.transform.LookAt(cameraPosition);
    }
>>>>>>> Stashed changes

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
<<<<<<< Updated upstream
            //Object is dead (call OnDeath method here)
            Destroy(Self);
=======
            Death();
>>>>>>> Stashed changes
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

<<<<<<< Updated upstream
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
=======
    public void Death()
    {
        Debug.Log(gameObject.name + " is dead!");

        Destroy(gameObject);
>>>>>>> Stashed changes
    }
}
