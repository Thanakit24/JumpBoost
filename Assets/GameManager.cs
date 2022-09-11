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
    public TMP_Text timeFinalDisplay;
    public TMP_Text jumpFinalCounterDisplay;
    public int jumpCounter;
    public bool levelFinish = false;

    public GameObject levelFinishUI;
    public GameObject inGameUI;
    public GameObject playerUI; 

    public Vector3 lastCheckPointPosition;
    //public FirstPersonPlayer player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        //DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        jumpCounter = 0;
        levelFinishUI.SetActive(false);
        inGameUI.SetActive(true);
        playerUI.SetActive(true);
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
            CompleteLevel();
        }

        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        timeDisplay.text = time.ToString(@"mm\:ss\:ff");

        jumpCounter = FirstPersonPlayer.instance.jumpGameCounter;
        jumpCounterDisplay.text = $"JUMP: {jumpCounter}";
       
        //time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
        //print(currentTime);
        //print("time counts");

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
    }
    public void ReloadScene()
    {
        //levelFinish = false;
        //levelFinishUI.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void CompleteLevel()
    {
        //print("player won");
        finishTime = currentTime;

        inGameUI.SetActive(false);
        playerUI.SetActive(false);

        Time.timeScale = 0f;
        jumpFinalCounterDisplay.text = $" {jumpCounter}";
        timeFinalDisplay.text = timeDisplay.text; 
        levelFinishUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadNextLevel()
    {
        print("load next lvl");
        //levelFinish = false;
        //levelFinishUI.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
