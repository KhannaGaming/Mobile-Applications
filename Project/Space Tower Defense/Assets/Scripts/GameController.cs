using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public byte Medals_Earned = 0;
    [HideInInspector]
    public byte Current_Medals = 0;
    [HideInInspector]
    public int Current_Gems = 0;
    [HideInInspector]
    public int Total_Gems_Earned = 0;
    [HideInInspector]
    public List<Level_Info> Level_Data = new List<Level_Info>();
    public int Health = 10;
    public GameObject EnemyPrefab;
    public Transform EnemySpawnLocation;
    // Use this for initialization
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

    }
    public void reduceHealth()
    {
        Health--;
    }

}

public class Level_Info
{
    public string Name = "";
    public byte Number = 0;
    private byte Medals
    {
        get
        {
            return this.Medals;
        }
        set
        {
            if (value < 0)
                this.Medals = 0;
            else if (value > 3)
                this.Medals = 3;
            else
                this.Medals = value;
        }
    }
}