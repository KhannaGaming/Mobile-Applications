using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    //This needs the Database Controller object dragged and dropped onto it:
    public Database_Control Database_Controller;

    public int Health = 10;
    public GameObject EnemyPrefab;
    public Transform EnemySpawnLocation;
    // Use this for initialization
    private void Awake()
    {

        //Use this as an example on how to save the game state:
        //The level names can be changed if you like, I've just set them to whatever turret is newly unlocked, but if you do change it let me know and I will update it in the database
        Database_Controller.GameState.Current_Medals = 50;
        Database_Controller.GameState.Current_Gems = 1000;
        Database_Controller.GameState.Total_Gems_Earned = 10000;
        Database_Controller.GameState.Total_Medals_Earned = 100;
        //Adding a level should only happen if the player has completed the level
        Database_Controller.GameState.Level_Data.Add(new Level_Info("Standard", 1, 3));
        Database_Controller.GameState.Level_Data.Add(new Level_Info("Lasers", 2, 3));
        Database_Controller.GameState.Level_Data.Add(new Level_Info("Electricity", 3, 3));
        Database_Controller.GameState.Level_Data.Add(new Level_Info("Sniper", 4, 3));
        Database_Controller.GameState.Level_Data.Add(new Level_Info("Rockets", 5, 3));
        Debug.Log("GameController Awake Initialisation Complete");
    }
    void Start()
    {
        Database_Controller.store.Load();
        Database_Controller.leaderboard.Load();
        Database_Controller.leaderboard.leaderboard(9001.0f, "Pavelow");
        Database_Controller.leaderboard.Load();
        Database_Controller.GameState.Load();
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
