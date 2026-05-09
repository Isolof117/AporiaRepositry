using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUps : MonoBehaviour
{
    // Weapon data from enemy
    private WeaponData data;

    private void Awake()
    {
        data = GetComponent<WeaponData>();

        if (data == null)
        {
            Debug.LogError("No WeaponData found on pickup prefab!");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides with the pickup
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player collided with pickup!");

        // Get the current weapon base from the pickup

        WeaponBase currentWeapon = other.GetComponentInChildren<WeaponBase>();

        if (currentWeapon == null)
        {
            Debug.LogError("Could not find player's WeaponBase!");
            return;
        }

        Debug.Log("Player weapon obtained!");

        // Get the weapon data from the enemy and apply it to the player's current weapon
        if (data != null)
        {
            currentWeapon.CancelQTE();
            data.SetData(currentWeapon);
            Debug.Log("Weapon data applied to player's current weapon");
            //currentWeapon.Reload();
        }

        currentWeapon.CancelQTE();
        //Destroy
        Destroy(gameObject);
    }
}
