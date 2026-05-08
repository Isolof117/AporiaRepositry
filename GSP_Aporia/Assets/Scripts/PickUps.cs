using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUps : MonoBehaviour
{
  
    private void OnTriggerEnter(Collider other)
    {
        //Do Something here

        Debug.Log("Something happened");


        //Destroy
        Destroy(gameObject);
    }
}
