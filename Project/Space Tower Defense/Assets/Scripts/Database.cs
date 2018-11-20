using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class Database : MonoBehaviour {

    public GameController gameController;

    public static Dictionary<string, float> Store = new Dictionary<string, float>();
    /// <summary>
    /// The Highscores are ordered from highest to lowest in the Dictionary automatically.
    /// </summary>
    public static Dictionary<string, float> Highscores = new Dictionary<string, float>();

    private MySqlConnection connection = null;
    private MySqlCommand command = null;
    private string connectionString = "Server = den1.mysql5.gear.host; port = 3306; Database = stddb; User = stdclient; Password = '8ch8J5PPRRCFKp6!';";
    private string DUI = "";
    private int Save_ID = 0;
    private byte Map_ID = 0;
    private string Map_Name = "";

    private float defaultRefreshTime = 5.0f; //Amount of seconds between database refreshes

    #region Query Database
    private MySqlDataReader queryDatabase(string query)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            command = new MySqlCommand(query, connection);
            return command.ExecuteReader();
        }
        catch (MySqlException SQLex)
        {
            Debug.Log("Database query failed (SQL Exception), query: " + query);
            Debug.Log("Exception Message: " + SQLex.Message);
            return null;
        }
        catch (System.Exception ex)
        {
            Debug.Log("Database query failed (SQL Exception), query: " + query);
            Debug.Log("Exception Message: " + ex.Message);
            return null;
        }
    }
    private bool? checkDatabase(string query)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            command = new MySqlCommand(query, connection);
            if (command.ExecuteReader().Read())
                return true;
            else
                return false;
        }
        catch (MySqlException SQLex)
        {
            Debug.Log("Database check failed (SQL Exception), query: " + query);
            Debug.Log("Exception Message: " + SQLex.Message);
            return null;
        }
        catch (System.Exception ex)
        {
            Debug.Log("Database check failed (System Exception), query: " + query);
            Debug.Log("Exception Message: " + ex.Message);
            return null;
        }
    }
    private T getDatabaseValue<T>(string value, string query)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            if (Equals(reader[value], typeof(T)))
                return (T)reader[value];
            else
                return default(T);
        }
        catch (MySqlException SQLex)
        {
            Debug.Log("Database get failed (SQL Exception), query: " + query);
            Debug.Log("Exception Message: " + SQLex.Message);
            return default(T);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Database get failed (System Exception), query: " + query);
            Debug.Log("Exception Message: " + ex.Message);
            return default(T);
        }
    }
    #endregion

    #region Store Data
    /// <summary>
    /// Refreshes the data dictionaries data with the current store items and prices.
    /// Items are sorted by their auto-generated ID.
    /// </summary>
    public void RefreshStoreData()
    {
        MySqlDataReader reader = null;
        reader = queryDatabase("SELECT Item_ID, Item_Name, Item_Price FROM tbl_Store WHERE In_Use = 1 ORDER BY Item_ID ASC;");
        if (reader == null)
        {
            //Query didn't return anything
            Debug.Log("RefreshStoreData query didn't return a value.");
        }
        else
        {
            Debug.Log("Refreshing store data.");
            Store.Clear();
            while (reader.Read())
            {
                Debug.Log(reader["Item_Name"].ToString() + " - " + (float)reader["Item_Price"]);
                Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
            }
        }
    }
    /// <summary>
    /// Get an items current store price by its name (Returns -1 if nothing was found).
    /// </summary>
    /// <param name="Item_Name">The name of the item you want the price for.</param>
    /// <returns>The price of the item passed via function parameters.</returns>
    public float GetItemPrice(string Item_Name)
    {
        MySqlDataReader reader = null;
        reader = queryDatabase("SELECT Item_Price FROM tbl_Store WHERE Item_Name = '" + Item_Name + "' AND In_Use = 1;");
        if (reader == null)
        {
            Debug.Log("GetItemPrice query didn't return a value.");
            return -1f;
        }
        else
        {
            Debug.Log("Returning requested item value.");
            return (float)reader["Item_Price"];
        }
    }
    #endregion

    #region Highscores Data
    /// <summary>
    /// Refreshes the data dictionaries data with the current highscores names and scores.
    /// The highscores do not need sorting, this is done by MySQL.
    /// </summary>
    public void RefreshHighscoresData()
    {
        MySqlDataReader reader = null;
        reader = queryDatabase("SELECT Player_Name, Highscore FROM tbl_Highscores ORDER BY Highscore ASC;");
        if (reader == null)
        {
            Debug.Log("RefreshHighscoresData didn't return a value.");
            //Query didn't return anything
        }
        else
        {
            Debug.Log("Refreshing Highscores data.");
            Highscores.Clear();
            while (reader.Read())
            {
                Debug.Log(reader["Player_Name"].ToString() + " - " + (float)reader["Highscore"]);
                Highscores.Add(reader["Player_Name"].ToString(), (float)reader["Highscore"]);
            }
        }
    }
        #region SubmitScore
        /// <summary>
        /// [float]
        /// Submit a highscore to the Highscores database table.
        /// Each player gets 1 record each, if the score submitted is higher than the currently logged one it is overwritten.
        /// </summary>
        /// <param name="Player_Name">Name of the player who submitted the score.</param>
        /// <param name="Score">The score achieved and wanting to be logged.</param>
        public void SubmitScore(string Player_Name, float Score)
        {
            MySqlDataReader reader = null;
            reader = queryDatabase("SELECT * FROM tbl_Highscores WHERE Player_Name = '" + Player_Name + "';");
            if (reader.Read())
            {
                if (Score > (float)reader["Highscore"])
                {
                    reader = queryDatabase("UPDATE tbl_Highscores SET Highscore = " + Score + " WHERE Player_Name = '" + Player_Name + "';");
                }
            }
            else
            {
                reader = queryDatabase("INSERT INTO tbl_Highscores (Player_Name, Highscore) SELECT '" + Player_Name + "', " + Score + ";");
            }
        }
        /// <summary>
        /// [int]
        /// Submit a highscore to the Highscores database table.
        /// Each player gets 1 record each, if the score submitted is higher than the currently logged one it is overwritten.
        /// </summary>
        /// <param name="Player_Name">Name of the player who submitted the score.</param>
        /// <param name="Score">The score achieved and wanting to be logged.</param>
        public void SubmitScore(string Player_Name, int Score)
        {
            MySqlDataReader reader = null;
            reader = queryDatabase("SELECT * FROM tbl_Highscores WHERE Player_Name = '" + Player_Name + "';");
            if (reader != null)
            {
                if (Score > (float)reader["Highscore"])
                {
                    reader = queryDatabase("UPDATE tbl_Highscores SET Highscore = " + (float)Score + " WHERE Player_Name = '" + Player_Name + "';");
                }
        }
        else
        {
            reader = queryDatabase("INSERT INTO tbl_Highscores (Player_Name, Highscore) SELECT '" + Player_Name + "', " + (float)Score + ";");
        }
    }
        #endregion
    #endregion

    #region Save Data
    public void SaveData()
    {
        if (DUI == "") DUI = SystemInfo.deviceUniqueIdentifier;
        try
        {
            //Check for existing save data in database:
            MySqlDataReader reader;
            int Save_ID = getDatabaseValue<int>("Save_ID", "SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
            bool? Level_Save_Check;
            bool? Map_Exists_Check;
            if (Save_ID > 0)
            {
                //Data
                Debug.Log("Database save data found, updating record.");
                reader = queryDatabase("UPDATE tbl_Save_Data " +
                    "SET Medals_Earned = " + gameController.Medals_Earned + ", " +
                        "Current_Medals = " + gameController.Current_Medals + ", " +
                        "Current_Gems = " + gameController.Current_Gems + ", " +
                        "Total_Gems_Earned = " + gameController.Total_Gems_Earned +
                    "WHERE Unique_Identifier = '" + DUI + ";");
                foreach (Level_Info level in gameController.Level_Data)
                {
                    //Check for save data for current level
                    Level_Save_Check = checkDatabase("SELECT * FROM tbl_Level_Save_Data " +
                        "LEFT JOIN tbl_Map_Data " +
                            "ON tbl_Map_Data.Map_ID = tbl_Level_Save_Data.Map_ID " +
                        "WHERE Save_ID = " + Save_ID + " AND " +
                            "(tbl_Map_Data.Level_Name = '" + level.Name + "' OR tbl_Map_Data.Map_ID = " + level.Number + ");");
                    if (Level_Save_Check == null)
                    {
                        Debug.Log("Level save check returned null, skipping save of level #" + level.Number + ": " + level.Name + " (" + level.Medals + ")");
                        continue;
                    }
                    //Check whether the map ID entered exists in the map data table, if not skip
                    Map_Exists_Check = checkDatabase("SELECT * FROM tbl_Map_Data WHERE Map_ID = " + level.Number + ";");
                    if (Map_Exists_Check == null || Map_Exists_Check == false)
                    {
                        Debug.Log("Map #" + level.Number + " does not exist, skipping save of level: " + level.Name + " (" + level.Medals + " Medals)");
                        continue;
                    }
                    else if (Level_Save_Check == true)
                        //Data found for item (update)
                        reader = queryDatabase("UPDATE tbl_Level_Save_Data " +
                            "SET Medals_Earned = " + level.Medals + " " +
                            "WHERE Save_ID = " + Save_ID + ";");
                    else
                        //No data found for item (insert)
                        reader = queryDatabase("INSERT INTO tbl_Level_Save_Data" +
                                "(Map_ID, Save_ID, Medals_Earned) " +
                            "SELECT " + level.Number + ", " + Save_ID + ", " + level.Medals + ";");
                }
            }
            else
            {
                //No data
                Debug.Log("Database save data not found, inserting new record.");
                reader = queryDatabase("INSERT INTO tbl_Save_Data " +
                        "(Medals_Earned, Current_Medals, Current_Gems, Total_Gems_Earned, Unique_Identifier) " +
                    "SELECT " + gameController.Medals_Earned + ", " +
                        gameController.Current_Medals + ", " +
                        gameController.Current_Gems + ", " +
                        gameController.Total_Gems_Earned + ", " +
                        "'" + DUI + "';");
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Database save failed: " + ex.Message + ", saving to dat file.");
        }
        //Save to bin file
        if (!File.Exists(Application.persistentDataPath + "/save.dat"))
            File.Create(Application.persistentDataPath + "/save.dat");
        FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, gameController.Medals_Earned);
            bf.Serialize(file, gameController.Current_Medals);
            bf.Serialize(file, gameController.Current_Gems);
            bf.Serialize(file, gameController.Total_Gems_Earned);
            foreach (Level_Info level in gameController.Level_Data)
            {
                bf.Serialize(file, level);
            }
        }
        catch (SerializationException e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            file.Close();
        }
    }
    public void LoadData()
    {
        //Check for existing save data in database
        //If found, load
        //Also load individual levels
        //If not found, check bin file
        if (File.Exists(Application.persistentDataPath + "/save.dat"))
        {

        }
    }
    private void GetMapID(string Level_Name)
    {
        MySqlDataReader reader = queryDatabase("SELECT Map_ID from tbl_Map_Data WHERE Level_Name = '" + Level_Name + "';");
        if (reader.Read())
            Map_ID = (byte)reader["Map_ID"];
    }
    private void GetMapName(byte Level_Number)
    {
        MySqlDataReader reader = queryDatabase("SELECT Level_Name FROM tbl_Map_Data WHERE Map_ID = " + Level_Number + ";");
        if (reader.Read())
            Map_Name = reader["Level_Name"].ToString();
    }
    private void GetSaveID()
    {
        MySqlDataReader reader = queryDatabase("SELECT Save_ID FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
        if (reader.Read())
            Save_ID = (int)reader["Save_ID"];
    }
    /// <summary>
    /// Saves the current overall game state.
    /// </summary>
    /// <param name="Medals_Earned">Lifetime medals earned from game so far.</param>
    /// <param name="Current_Medals">Current amount of medals in "inventory", this can be the same as Medals_Earned, it's just here in case we need to add additional store functionality.</param>
    /// <param name="Current_Gems">Current amount of ingame currency available.</param>
    /// <param name="Total_Gems_Earned">Lifetime amount of ingame currency earned.</param>
    private void SaveGame(byte Medals_Earned, byte Current_Medals, int Current_Gems, int Total_Gems_Earned)
    {
        MySqlDataReader reader = null;
        reader = queryDatabase("SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
        if (reader.Read())
        {
            Save_ID = (int)reader["Save_ID"];
            reader = queryDatabase("UPDATE tbl_Save_Data " +
                "SET Medals_Earned = " + Medals_Earned +
                    "Current_Medals = " + Current_Medals +
                    "Current_Gems = " + Current_Gems +
                    "Total_Gems_Earned = " + Total_Gems_Earned +
                "WHERE Unique_Identifier = '" + DUI + "';");
        }
        else
        {
            reader = queryDatabase("INSERT INTO tbl_Save_Data " +
                "(Medals_Earned, Current_Medals, Current_Gems, Total_Gems_Earned, Unique Identifier) " +
                "SELECT " +
                    Medals_Earned + ", " +
                    Current_Medals + ", " +
                    Current_Gems + ", " +
                    Total_Gems_Earned + ", " +
                    "'" + DUI + "';");
            reader = queryDatabase("SELECT Save_ID FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
            reader.Read();
            Save_ID = (int)reader["Save_ID"];
        }
    }
    private void SaveLevel(string Level_Name, byte Medals_Earned)
    {
        MySqlDataReader reader = null;
        byte Map_ID = 0;
        //Get the map ID
        reader = queryDatabase("SELECT * FROM tbl_Map_Data WHERE Level_Name = '" + Level_Name + "';");
        if (reader.Read())
        {
            //Map ID found
            Map_ID = (byte)reader["Map_ID"];
            Debug.Log("Found Map_ID for " + Level_Name + " (Map_ID: " + Map_ID + ")");
        }
        else
        {
            //Map ID not found, exit
            Debug.Log("Saving level " + Level_Name + " failed, please check the spelling (Map_ID could not be found).");
            return;
        }
        //If we don't have the current save_id, get it
        if (Save_ID == 0)
            GetSaveID();
        //Check to see if there is a level save for this level currently
        reader = queryDatabase("SELECT * FROM tbl_Level_Save_Data WHERE Save_ID = " + Save_ID + " AND Map_ID = " + Map_ID + ";");
        if (reader.Read())
        {
            //Level save found
            reader = queryDatabase("UPDATE tbl_Level_Save_Data SET Medals_Earned = " + Medals_Earned +
                " WHERE Save_ID = " + Save_ID + " AND Map_ID = " + Map_ID + ";");
        }
        else
        {
            //Level save not found (create one)
            reader = queryDatabase("INSERT INTO tbl_Level_Save_Data " +
                "(Map_ID, Save_ID, Medals_Earned) " +
                "SELECT " + Map_ID + ", " + Save_ID + ", " + Medals_Earned + ";");
        }
    }
    #endregion

    #region General Initialisation
    private void Awake()
    {
        DUI = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Unique ID: " + DUI);
        //RefreshHighscoresData();
        //RefreshStoreData();
        SaveData();
        LoadData();
    }
    #endregion

    //#region Coroutine Initialisation
    //private void Start()
    //{
    //    StartCoroutine(RefreshData());
    //}

    //IEnumerator RefreshData()
    //{
    //    yield return new WaitForSeconds(defaultRefreshTime);

    //    //Refresh DB data

    //    RefreshStoreData();
    //    RefreshHighscoresData();

    //    StartCoroutine(RefreshData());
    //}
    //#endregion
}
