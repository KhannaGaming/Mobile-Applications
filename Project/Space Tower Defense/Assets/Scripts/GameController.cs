using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GameController : MonoBehaviour {

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
    public int Health = 10;
    public Transform EnemySpawnLocation;

    private string LevelData;
    private int currentWaveNumber;
    private int StandardEnemies = 0;
    private int SlowEnemies = 0;
    private int StealthyEnemies = 0;
    private int FastEnemies = 0;
    // Use this for initialization
    void Start () {
        ReadLevelData();
        ReadWaveData(1);
        CreateEnemies();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void reduceHealth()
    {
        Health--;
    }

    [MenuItem("Tools/Read file")]
    void ReadLevelData()
    {
        string path = "Assets/Resources/test.txt";

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
            StandardEnemies = b[2 + (waveNumber*10)] - '0';
            SlowEnemies = b[4 + (waveNumber * 10)] - '0';
            FastEnemies = b[6 + (waveNumber * 10)] - '0';
            StealthyEnemies = b[8 + (waveNumber * 10)] - '0';
        }
    }
    void CreateEnemies()
    {
        for (int i = 0; i < StandardEnemies; i++)
        {
            Instantiate(StandardEnemyPrefab, EnemySpawnLocation.position, EnemySpawnLocation.rotation);
        }
        for (int i = 0; i < SlowEnemies; i++)
        {
            Instantiate(SlowEnemyPrefab, EnemySpawnLocation.position, EnemySpawnLocation.rotation);
        }
        for (int i = 0; i < StealthyEnemies; i++)
        {
            Instantiate(FastEnemyPrefab, EnemySpawnLocation.position, EnemySpawnLocation.rotation);
        }
        for (int i = 0; i < FastEnemies; i++)
        {
            Instantiate(StealthyEnemyPrefab, EnemySpawnLocation.position, EnemySpawnLocation.rotation);
        }
    }
}
