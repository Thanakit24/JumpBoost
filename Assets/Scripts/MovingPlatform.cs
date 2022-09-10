using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform[] positions;
    Rigidbody rb;
    int currentWaypointIndex = 0;
    [SerializeField] float moveSpeed;

    private void Start()
    {
        //rb = GetComponent<Rigidbo>
        transform.position = positions[1].transform.position;
    }
    void Update()
    {
        if (Vector3.Distance(transform.position, positions[currentWaypointIndex].position) <= 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= positions.Length)
            {
                currentWaypointIndex = 0;
            }
        }
        //print("Move platform");
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, positions[currentWaypointIndex].transform.position, moveSpeed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            VolumeManager.instance.OffMotionBlur();
            other.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            VolumeManager.instance.OnMotionBlur();
            other.transform.SetParent(null);
        }
    }
}
