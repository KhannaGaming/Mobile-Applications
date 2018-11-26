using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [Header("Resources")]

    [Header("Enemey")]
    public List<GameObject> Enemies;
    public List<GameObject> EnemiesHealth;

    // public List<GameObject> Enemies2;
    [Header("Player Base")]
    public int Health = 10;
    public GameObject EnemyPrefab;
    public Transform EnemySpawnLocation;
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
        for (int i = 0; i < 15; i++)
        {
            Instantiate(EnemyPrefab, EnemySpawnLocation.position, EnemySpawnLocation.rotation);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        PopulateEnemyList();
        //RemoveEnemies();  //Obselete
        OrderEnemies();
        PopulateEnemyListStrongest();


    }

    private void RemoveEnemies()
    {
        Enemies.RemoveAll(enemies => enemies == null);
    }

    public void reduceHealth()
    {
        Health--;
    }

    public void OrderEnemies()
    {

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

    public void PopulateEnemyListStrongest()
    {
        EnemiesHealth = Enemies.OrderBy(s => s.gameObject.GetComponent<EnemyController>().health).ToList();
    }


}
