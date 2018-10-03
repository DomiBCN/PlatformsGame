﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [SerializeField]
    PlayerController player;
    [SerializeField]
    Text recordText;
    [SerializeField]
    Button startButton;
    [SerializeField]
    string level;
    [SerializeField]
    Transform levelContainer;
    [SerializeField]
    List<GameObject> levels = new List<GameObject>();
    
    [SerializeField]
    GameObject pauseMenu;

    int currentLevel;
    Rigidbody2D rigidBody;
    int secondsToStart = 3;
    Text mainText;
    float initialTime;
    float bestTime;
    float finalTime;
    bool paused = false;

    private void Awake()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        GameObject levelPrefab = Instantiate(levels[currentLevel]);
        levelPrefab.transform.SetParent(levelContainer);
    }

    // Use this for initialization
    void Start()
    {
        rigidBody = player.GetComponent<Rigidbody2D>();
        player.eliminated += Restart;
        player.levelEnd += End;
        player.enabled = false;
        mainText = startButton.GetComponentInChildren<Text>();
        bestTime = GetBestTime(currentLevel);
        if (bestTime > 0) { recordText.text = "Record: " + bestTime.ToString("##.##") + " s"; } else { recordText.enabled = false; }
        //DEBUGGING
        //StartGame();
    }

    public void Pause()
    {
        if (paused)
        {
            pauseMenu.SetActive(false);
            setTimeScale(1);
        }
        else
        {
            setTimeScale(0);
            pauseMenu.SetActive(true);
        }
        paused = !paused;
    }

    public void Restart()
    {
        if (paused)
        {
            //If Restart from pause menu, we need to set timeScale to 1. It's value doesn't reset on loadScene
            setTimeScale(1);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        //If we Exit from pause menu, we need to set timeScale to 1. It's value doesn't reset on loadScene
        setTimeScale(1);
        SceneManager.LoadScene("LevelMenu");
    }

    public void StartGame()
    {
        startButton.enabled = false;
        mainText.text = "" + secondsToStart;
        //DEBUGGING
        //GameStarted();
        InvokeRepeating("CountDown", 1, 1);
        
    }

    void CountDown()
    {
        secondsToStart--;
        if (secondsToStart <= 0) { CancelInvoke(); GameStarted(); } else { mainText.text = secondsToStart.ToString(); }
    }

    void GameStarted()
    {
        player.enabled = true;
        initialTime = Time.time;
        if (bestTime > 0) recordText.enabled = true;
    }

    void FixedUpdate()
    {
        if (player.enabled)
        {
            mainText.text = "Tiempo: " + Math.Round((Time.time - initialTime), 2).ToString("##.##");
        }
    }

    void End()
    {
        player.enabled = false;
        setTimeScale(0);
        finalTime = (Time.time - initialTime);
        mainText.text = "Final! " + Math.Round(finalTime, 2);
        if (finalTime < bestTime || bestTime == 0) SetRecord(currentLevel, finalTime);
    }

    public float GetBestTime(int level)
    {
        return PlayerPrefs.GetFloat(level + "_best", 0);
    }

    public void SetRecord(int level, float record)
    {
        PlayerPrefs.SetFloat(level + "_best", record);
    }

    void setTimeScale(int timeScale)
    {
        Time.timeScale = timeScale;
    }
}
