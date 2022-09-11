using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        //check if player hit the platform
        //check if player has collided with checkpoints before or not
        //if above is true then set player position to checkpoint position after an animation?
        //dont destroy checkpoint position, make it invisible 
    }

    // Update is called once per frame
    void Update()
    {
        //if (player.isDead)
        //{
        //    player.transform.position = lastCheckPointPosition;
        //    player.isDead = false;
        //}
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //FirstPersonPlayer player = other.gameObject.GetComponent<FirstPersonPlayer>();
            print("Hit dead zone");
            //FirstPersonPlayer.instance.isDead = true;
            FirstPersonPlayer.instance.gameObject.transform.position = GameManager.instance.lastCheckPointPosition;
            FirstPersonPlayer.instance.jumpChargeBar.value = 0;
            FirstPersonPlayer.instance.currentJumpForce = FirstPersonPlayer.instance.defaultJumpForce;
        }
    }
}
