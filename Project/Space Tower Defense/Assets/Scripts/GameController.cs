using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public int Health = 10;
    public GameObject EnemyPrefab;
    public Transform EnemySpawnLocation;
	// Use this for initialization
	void Start () {
        for (int i = 0; i < 15; i++)
        {
            Instantiate(EnemyPrefab, EnemySpawnLocation.position, EnemySpawnLocation.rotation);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void reduceHealth()
    {
        Health--;
    }
}
