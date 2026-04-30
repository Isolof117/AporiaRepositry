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
        if(Input.GetKeyDown(KeyCode.H))
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
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            //Object is dead (call OnDeath method here)

        }
        healthSlider.value = currentHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
