using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    private float startTime = 0f;
    private float currentTime;
    public float finishTime;

    public TMP_Text timeDisplay;
    public TMP_Text jumpCounterDisplay;
    public int jumpCounter;
    [SerializeField] private bool levelFinish = false;

    public FirstPersonPlayer player;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = startTime;

    }

    // Update is called once per frame
    void Update()
    {
        if (!levelFinish)
        {
            currentTime += 1 * Time.deltaTime;
        }

        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        timeDisplay.text = time.ToString(@"mm\:ss\:ff");

        jumpCounter = player.jumpGameCounter;

        jumpCounterDisplay.text = jumpCounter.ToString("JUMPS: " + jumpCounter);
            //time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
        //print(currentTime);
        //print("time counts");
        
    }
}
