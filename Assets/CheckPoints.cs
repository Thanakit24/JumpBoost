using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    //public DeadZone deadZone;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("oekkwoekwoe");
            GameManager.instance.lastCheckPointPosition = transform.position;
            Destroy(gameObject);
        }
       
    }
}
