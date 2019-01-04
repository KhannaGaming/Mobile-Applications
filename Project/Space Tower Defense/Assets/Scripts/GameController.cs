﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
   // [Header("Database")]
    //This needs the Database Controller object dragged and dropped onto it:
    //public Database_Control Database_Controller;

    [Header("StandardEnemy")]
    public GameObject StandardEnemyPrefab;
    [Header("SlowEnemy")]
    public GameObject SlowEnemyPrefab;
    [Header("StealthyEnemy")]
    public GameObject StealthyEnemyPrefab;
    [Header("FastEnemy")]
    public GameObject FastEnemyPrefab;
    [Header("Other")]
    [Tooltip("Base Health")]

    [Header("Resources")]

    // public List<GameObject> Enemies2;
    [Header("Player Base")]

    public int Health = 10;
    public List<Transform> EnemySpawnLocations;

   // public Transform EnemySpawnLocation;

    [Header("Canvas")]
    public GameObject NextWaveButton;

    private string LevelData;
    private int currentWaveNumber = 0;
    private int StandardEnemies = 0;
    private int SlowEnemies = 0;
    private int StealthyEnemies = 0;
    private int FastEnemies = 0;
    private int maxWaveNumber;
    public GameObject CurrentEnemy = null;
    public bool wait = true;

    public static GameController Instance { get; private set; }

    public List<GameObject> Enemies = null;
    public GameObject waveController;

    private List<List<GameObject>> pathsPossible;
    public List<GameObject> firstPathPossible;
    public List<GameObject> secondPathPossible;

    // Use this for initialization
    public void Awake()
    {
        
        ////Database_Controller.LoadData();
        //if (Instance == null)                   //if this instnace hasn't been made yet
        //{
        //    Instance = this;                    //Make instance this
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);                //Destroy duplicates 
        //}
        //Database_Controller.GameState.Current_Medals = 10;
        //Database_Controller.GameState.Current_Gems = 2;
        //Database_Controller.GameState.Total_Medals_Earned = 28;
        //Database_Controller.GameState.Total_Gems_Earned = 38;
        //Database_Controller.GameState.Level_Data.Add(new Level_Info("Level01", 1, 3));
        ////Dictionary<string,float> temp_Leaderboard = Database_Controller.Leaderboard(10032.0f, "KhannaGaming");
        //Database_Controller.SaveData();
        //Database_Controller.LoadData();
    }

    void Start()
    {
        //for testing only
        currentWaveNumber = 0;
        ReadLevelData();
        pathsPossible = new List<List<GameObject>>();
        pathsPossible.Add(firstPathPossible);
        pathsPossible.Add(secondPathPossible);
        //ReadNextWaveData(currentWaveNumber);
    }

    private void Update()
    {
        if (Health <= 0)
        {
            PlayerPrefs.SetString("Level", "MainMenu");
            SceneManager.LoadScene("Loading");
        }
    }
    public void reduceHealth()
    {
        Health--;
    }

    
    void ReadLevelData()
    {
        string path = "Assets/Resources/Level"+PlayerPrefs.GetInt("LevelNumber",1)+".txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        LevelData = reader.ReadToEnd();
        reader.Close();
        ReadNextWaveData(1);
    }

    bool ReadNextWaveData(int waveNumber)
    {
        char[] b = new char[LevelData.Length];
        if (waveNumber <= 5)
        {
            using (StringReader sr = new StringReader(LevelData))
            {
                sr.Read(b, 0, LevelData.Length);
                maxWaveNumber = 0;
                for (int i = 1; i < LevelData.Length; i++)
                {
                    
                    if (b[i] == '\n' && (b[i + 1] - '0') == waveNumber)
                    {
                        StandardEnemies = (((b[i + 3] - '0') * 10) + (b[i + 4]) - '0');
                        SlowEnemies = (((b[i + 6] - '0') * 10) + (b[i + 7]) - '0');
                        FastEnemies = (((b[i + 9] - '0') * 10) + (b[i + 10]) - '0');
                        StealthyEnemies = (((b[i + 12] - '0') * 10) + (b[i + 13]) - '0');
                    }
                    if (b[i] == '\n')
                    {
                        maxWaveNumber++;
                    }
                }
                waveController.GetComponent<WaveController>().maxWave(maxWaveNumber-1);
            }
            return true;
        }
        else
        {
            StandardEnemies = 0;
            SlowEnemies = 0;
            FastEnemies = 0;
            StealthyEnemies = 0;
            return false;
        }
}

    void CreateEnemies()
    {
        InvokeRepeating("CreateStandardEnemies", 0f, 1.0f);       
    }

   


    void CreateStandardEnemies()
    {
        if (StandardEnemies > 0)
        {
            int SpawanLocation = UnityEngine.Random.Range(0, EnemySpawnLocations.Count);
            CurrentEnemy = Instantiate(StandardEnemyPrefab, EnemySpawnLocations[SpawanLocation].position, EnemySpawnLocations[SpawanLocation].rotation);
            CurrentEnemy.GetComponent<EnemyController>().setPath(pathsPossible[SpawanLocation]);
            AddEnemy(CurrentEnemy);
        }
        if(--StandardEnemies <=0)
        {
            CancelInvoke("CreateStandardEnemies");
            InvokeRepeating("CreateSlowEnemies",0.0f,0.8f);
        }
    }

    void CreateSlowEnemies()
    {
        if (SlowEnemies > 0)
        {
            int SpawanLocation = UnityEngine.Random.Range(0, EnemySpawnLocations.Count);
            CurrentEnemy = Instantiate(SlowEnemyPrefab, EnemySpawnLocations[SpawanLocation].position, EnemySpawnLocations[SpawanLocation].rotation);
            CurrentEnemy.GetComponent<EnemyController>().setPath(pathsPossible[SpawanLocation]);
            AddEnemy(CurrentEnemy);
        }
        if (--SlowEnemies <= 0)
        {
            CancelInvoke("CreateSlowEnemies");
            InvokeRepeating("CreateStealthyEnemies", 0f, 0.5f);
            
        }
    }

    void CreateStealthyEnemies()
    {
        if (StealthyEnemies > 0)
        {
            int SpawanLocation = UnityEngine.Random.Range(0, EnemySpawnLocations.Count);
            CurrentEnemy = Instantiate(StealthyEnemyPrefab, EnemySpawnLocations[SpawanLocation].position, EnemySpawnLocations[SpawanLocation].rotation);
            CurrentEnemy.GetComponent<EnemyController>().setPath(pathsPossible[SpawanLocation]);
            AddEnemy(CurrentEnemy);
        }
        if (--StealthyEnemies <= 0)
        {
            CancelInvoke("CreateStealthyEnemies");
            InvokeRepeating("CreateFastEnemies", 0f, 1.2f);
        }
    }

    void CreateFastEnemies()
    {
        if (FastEnemies > 0)
        {
            int SpawanLocation = UnityEngine.Random.Range(0, EnemySpawnLocations.Count);
            CurrentEnemy = Instantiate(FastEnemyPrefab, EnemySpawnLocations[SpawanLocation].position, EnemySpawnLocations[SpawanLocation].rotation);
            CurrentEnemy.GetComponent<EnemyController>().setPath(pathsPossible[SpawanLocation]);
            AddEnemy(CurrentEnemy);
        }

        if (--FastEnemies <= 0)
        {
            CancelInvoke("CreateFastEnemies");
            NextWaveButton.GetComponent<Button>().interactable = true;
            waveController.GetComponent<WaveController>().lastEnemySent = true;
        }
    }
    public void NextWave()
    {
        waveController.GetComponent<WaveController>().increaseWave();
        NextWaveButton.GetComponent<Button>().interactable = false;

        currentWaveNumber++;
        if (!ReadNextWaveData(currentWaveNumber))
        {
            EndOfWaves();
        }
        CreateEnemies();
    }

    void EndOfWaves()
    {

    }

    public void AddEnemy(GameObject Enemy)
    {
        Enemies.Add(Enemy);
    }
    
    public int checkEnemies()
    {
        int enemiesLeft = 0;
        for (int i = 0; i < Enemies.Count; i++)
        {
            if(Enemies[i] != null)
            {
                enemiesLeft++;
            }
        }
        return enemiesLeft;
    }
    
}
