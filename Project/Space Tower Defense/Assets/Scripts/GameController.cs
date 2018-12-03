using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GameController : MonoBehaviour
{

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

    [Header("Enemey")]
    public List<GameObject> Enemies;
    public List<GameObject> EnemiesReversed;
    public List<GameObject> EnemiesHealth;
    public List<GameObject> EnemiesHealthReversed;

    // public List<GameObject> Enemies2;
    [Header("Player Base")]

    public int Health = 10;
    public Transform EnemySpawnLocation;


    private string LevelData;
    private int currentWaveNumber = 0;
    private int StandardEnemies = 0;
    private int SlowEnemies = 0;
    private int StealthyEnemies = 0;
    private int FastEnemies = 0;
   
	

    public GameObject CurrentEnemy = null;
    public bool wait = true;

    public static GameController Instance { get; private set; }

    // Use this for initialization
    public void Awake()
    {   
        if (Instance == null)                   //if this instnace hasn't been made yet
        {
            Instance = this;                    //Make instance this
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);                //Destroy duplicates 
        }
    }

    void Start()
    {
        //for testing only
        currentWaveNumber = 1;
        ReadLevelData();
        ReadWaveData(currentWaveNumber);
        //CreateEnemies();
        InvokeRepeating("CreateStandardEnemies", 0f, 1.0f);



    }

    // Update is called once per frame
    void Update()
    {
        OrderEnemies();  
    }

    private void RemoveEnemies()
    {
        Enemies.RemoveAll(enemies => enemies == null);
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
    }

    void ReadWaveData(int waveNumber)
    {
        char[] b = new char[LevelData.Length];

        using (StringReader sr = new StringReader(LevelData))
        {
            sr.Read(b, 0, LevelData.Length);

            for (int i = 0; i < LevelData.Length; i++)
            {
                
                if (b[i] == '\n' && (b[i + 1] - '0') == waveNumber)
                {
                    StandardEnemies = (((b[i + 3]-'0')*10)+ (b[i + 4]) - '0');
                    SlowEnemies = (((b[i + 6] - '0') * 10) + (b[i + 7]) - '0');
                    FastEnemies = (((b[i + 9] - '0') * 10) + (b[i + 10]) - '0');
                    StealthyEnemies = (((b[i + 12] - '0') * 10) + (b[i + 13]) - '0');
                }
            }
        }
    }

    void CreateEnemies()
    {
        //for (int i = 0; i < StandardEnemies; i++)
        //{
            InvokeRepeating("CreateStandardEnemies", 0f, 1.0f);
        //}
        for (int i = 0; i < SlowEnemies; i++)
        {
            CurrentEnemy = Instantiate(SlowEnemyPrefab, EnemySpawnLocation.position, EnemySpawnLocation.rotation);
            CurrentEnemy.name = "SlowEnemy " + i;
        }
        for (int i = 0; i < StealthyEnemies; i++)
        {
            CurrentEnemy = Instantiate(FastEnemyPrefab, EnemySpawnLocation.position, EnemySpawnLocation.rotation);
            CurrentEnemy.name = "FastEnemy " + i;
        }
        for (int i = 0; i < FastEnemies; i++)
        {
            CurrentEnemy = Instantiate(StealthyEnemyPrefab, EnemySpawnLocation.position, EnemySpawnLocation.rotation);
            CurrentEnemy.name = "StealthyEnemy " + i;
        }
    }


    public void OrderEnemies()
    {
        PopulateEnemyList();
        PopulateEnemyListReversed();
        PopulateEnemyListStrongest();
        PopulateEnemyListWeakest();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.GetComponent<EnemyController>().inList == false)
            {
                Enemies.Add(other.gameObject);
                other.GetComponent<EnemyController>().inList = true;
            }
           
            //Debug.Log("Enemies in scene");
            
        }
    }

    public void PopulateEnemyList()
    {
        Enemies = Enemies.OrderByDescending(s => s.gameObject.GetComponent<EnemyController>().currentPathNode).ThenBy(s => s.gameObject.GetComponent<EnemyController>().distanceLeft).ToList();

        int counter = 1; 
        foreach (GameObject enemy in Enemies)
        {
            enemy.GetComponent<EnemyController>().currentPlace = counter;
            counter++;
        } 
    }

    public void PopulateEnemyListReversed()
    {
        EnemiesReversed = Enemies.OrderBy(s => s.gameObject.GetComponent<EnemyController>().currentPathNode).ThenByDescending(s => s.gameObject.GetComponent<EnemyController>().distanceLeft).ToList();

        int counter = 1;
        foreach (GameObject enemy in Enemies)
        {
            enemy.GetComponent<EnemyController>().currentPlace = counter;
            counter++;
        }
    }

    public void PopulateEnemyListWeakest()
    {
        EnemiesHealth = Enemies.OrderBy(s => s.gameObject.GetComponent<EnemyController>().health).ToList();
    }

    public void PopulateEnemyListStrongest()
    {
        EnemiesHealthReversed = Enemies.OrderByDescending(s => s.gameObject.GetComponent<EnemyController>().health).ToList();
    }

    void CreateStandardEnemies()
    {

        CurrentEnemy = Instantiate(StandardEnemyPrefab, EnemySpawnLocation.position, EnemySpawnLocation.rotation);
            //CurrentEnemy.name = "StandardEnemy " + i;
        if(--StandardEnemies ==0)
        {
            CancelInvoke("CreateStandardEnemies");
        }
    }

}
