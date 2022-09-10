using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private float startTime = 0f;
    private float currentTime;
    public float finishTime;

    public TMP_Text timeDisplay;
    public TMP_Text jumpCounterDisplay;
    public int jumpCounter;
    [SerializeField] private bool levelFinish = false;

    public Vector3 lastCheckPointPosition;
    //public FirstPersonPlayer player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        

    }
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
        else
        {
            finishTime = currentTime;
        }

        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        timeDisplay.text = time.ToString(@"mm\:ss\:ff");

        jumpCounter = FirstPersonPlayer.instance.jumpGameCounter;

        jumpCounterDisplay.text = jumpCounter.ToString("JUMPS: " + jumpCounter);
            //time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
        //print(currentTime);
        //print("time counts");

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
    }
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
