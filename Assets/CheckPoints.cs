using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    //public DeadZone deadZone;
    public float speed;

    private void Update()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f, Space.Self);
    }
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
