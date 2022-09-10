using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform[] positions;
    int currentWaypointIndex = 0;
    [SerializeField] float moveSpeed;

    void Update()
    {
        if (Vector3.Distance(transform.position, positions[currentWaypointIndex].position) < .1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= positions.Length)
            {
                currentWaypointIndex = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, positions[currentWaypointIndex].transform.position, moveSpeed * Time.deltaTime);
        //print("Move platform");

    } 
       
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //print("Set player parent");
            other.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
