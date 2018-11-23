using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public DateTime Modified = new DateTime();
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
    private void Awake()
    {
        //Use this as an example on how to save the game state:
        //The level names can be changed if you like, I've just set them to whatever turret is newly unlocked, but if you do change it let me know and I will update it in the database
        Modified = DateTime.Now;
        Medals_Earned = 100;
        Current_Medals = 50;
        Current_Gems = 1000;
        Total_Gems_Earned = 10000;
        //Adding a level should only happen if the player has completed the level
        Level_Data.Add(new Level_Info("Standard", 1, 3));
        Level_Data.Add(new Level_Info("Lasers", 2,3));
        Level_Data.Add(new Level_Info("Electricity", 3, 3));
        Level_Data.Add(new Level_Info("Sniper", 4, 3));
        Level_Data.Add(new Level_Info("Rockets", 5, 3));
        Debug.Log("GameController Awake Initialisation Complete");
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

    }
    public void reduceHealth()
    {
        Health--;
    }

}

[Serializable]
public class Level_Info
{
    public string Name = "";
    public byte Number = 0;
    public byte Medals = 1;
    /// <summary>
    /// Constructor for Level_Info
    /// </summary>
    /// <param name="Name_">The name of the level being saved</param>
    /// <param name="Number_">The number of the level being saved</param>
    /// <param name="Medals_">The number of medals being saved (1 to 3)</param>
    public Level_Info(string Name_, byte Number_, byte Medals_)
    {
        Name = Name_;
        Number = Number_;
        Medals = Medals_;
    }
}