using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas canvas;

    [SerializeField] private Transform cameraPosition;


    private void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
    private void Update()
    {
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

    public void TakeDamage(int damage)
    {
        Debug.Log($"Before Damage: {currentHealth}");

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);


        Debug.Log($"After Damage: {currentHealth}");

        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void Death()
    {
        Debug.Log(gameObject.name + " is dead!");

        Destroy(gameObject);
    }
}
