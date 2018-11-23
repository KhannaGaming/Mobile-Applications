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
using System;

public class Database : MonoBehaviour {

    public static GameController gameController;
    private Debug_Log d = new Debug_Log();
    private Local_Cache LC = new Local_Cache(gameController);

    #region Database Variables
    private MySqlConnection connection = null;
    private MySqlCommand command = null;
    private string connectionString = "Server = den1.mysql5.gear.host; port = 3306; Database = stddb; User = stdclient; Password = '8ch8J5PPRRCFKp6!';";
    #endregion
    #region Save State Variables
    private string DUI = "";
    [HideInInspector]
    public int Save_ID = 0;
    private byte Map_ID = 0;
    private string Map_Name = "";
    #endregion

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
            if (reader.Read())
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
        MySqlDataReader reader = queryDatabase("SELECT Item_ID, Item_Name, Item_Price FROM tbl_Store WHERE In_Use = 1 ORDER BY Item_ID ASC;");
        if (reader == null)
            d.Log("Database.RefreshStoreData","RefreshStoreData query didn't return a value.",true);
        else
            LC.Save(reader, 2);
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
        MySqlDataReader reader = queryDatabase("SELECT Player_Name, Score FROM tbl_Highscores ORDER BY Score ASC;");
        if (reader == null)
            d.Log("Database.RefreshHighscoresData","RefreshHighscoresData didn't return a value.",true);
        else
            LC.Save(reader, 1);
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
                if (Score > (float)reader["Score"])
                {
                    reader = queryDatabase("UPDATE tbl_Highscores SET Score = " + Score + " WHERE Player_Name = '" + Player_Name + "';");
                }
            }
            else
            {
                reader = queryDatabase("INSERT INTO tbl_Highscores (Player_Name, Score) SELECT '" + Player_Name + "', " + Score + ";");
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
            if (Score > (float)reader["Score"])
            {
                reader = queryDatabase("UPDATE tbl_Highscores SET Score = " + (float)Score + " WHERE Player_Name = '" + Player_Name + "';");
            }
        }
        else
        {
            reader = queryDatabase("INSERT INTO tbl_Highscores (Player_Name, Score) SELECT '" + Player_Name + "', " + (float)Score + ";");
        }
    }
        #endregion
    #endregion

    #region Save Data
    public void SaveData()
    {
        if (DUI == "") DUI = SystemInfo.deviceUniqueIdentifier;
        //Save to database
        try
        {
            //Check for existing save data in database:
            MySqlDataReader reader;
            Save_ID = getDatabaseValue<int>("Save_ID", "SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
            bool? Level_Save_Check;
            bool? Map_Exists_Check;
            if (Save_ID > 0)
            {
                //Data
                d.Log("Database save data found, updating record.",false);
                reader = queryDatabase("UPDATE tbl_Save_Data " +
                    "SET Medals_Earned = " + gameController.Medals_Earned + ", " +
                        "Current_Medals = " + gameController.Current_Medals + ", " +
                        "Current_Gems = " + gameController.Current_Gems + ", " +
                        "Total_Gems_Earned = " + gameController.Total_Gems_Earned + ", " +
                        "Modified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                    "WHERE Unique_Identifier = '" + DUI + "';");
                foreach (Level_Info level in gameController.Level_Data)
                {
                    //Check for save data for current level
                    Level_Save_Check = checkDatabase("SELECT * FROM tbl_Level_Save_Data " +
                        "LEFT JOIN tbl_Map_Data " +
                            "ON tbl_Map_Data.Level_Number = tbl_Level_Save_Data.Map_ID " +
                        "WHERE Save_ID = " + Save_ID + " AND " +
                            "(tbl_Map_Data.Level_Name = '" + level.Name + "' OR tbl_Map_Data.Level_Number = " + level.Number + ");");
                    if (Level_Save_Check == null)
                    {
                        d.Log("Level save check returned null, skipping save of level #" + level.Number + ": " + level.Name + " (" + level.Medals + ")",false);
                        continue;
                    }
                    //Check whether the map ID entered exists in the map data table, if not skip
                    Map_Exists_Check = checkDatabase("SELECT * FROM tbl_Map_Data WHERE Level_Number = " + level.Number + ";");
                    if (Map_Exists_Check == null || Map_Exists_Check == false)
                    {
                        d.Log("Map #" + level.Number + " does not exist, skipping save of level: " + level.Name + " (" + level.Medals + " Medals)",false);
                        continue;
                    }
                    else if (Level_Save_Check == true && Map_Exists_Check == true)
                    {
                        //Data found for item (update)
                        d.Log("Data found for level save #" + level.Number + " Medals: " + level.Medals + " Number: " + level.Number, false);
                        reader = queryDatabase("UPDATE tbl_Level_Save_Data " +
                                "SET Medals_Earned = " + level.Medals + " " +
                                "WHERE Save_ID = " + Save_ID + ";");
                    }
                    else
                    {
                        //No data found for item (insert)
                        d.Log("Data not found for level save #" + level.Number + " Medals: " + level.Medals + " Name: " + level.Name, false);
                        reader = queryDatabase("INSERT INTO tbl_Level_Save_Data" +
                                "(Map_ID, Save_ID, Medals_Earned) " +
                            "SELECT " + level.Number + ", " + Save_ID + ", " + level.Medals + ";");
                    }
                }
            }
            else
            {
                //No data
                d.Log("Database save data not found, inserting new record.",false);
                reader = queryDatabase("INSERT INTO tbl_Save_Data " +
                        "(Medals_Earned, Current_Medals, Current_Gems, Total_Gems_Earned, Unique_Identifier, Modified) " +
                    "SELECT " + gameController.Medals_Earned + ", " +
                        gameController.Current_Medals + ", " +
                        gameController.Current_Gems + ", " +
                        gameController.Total_Gems_Earned + ", " +
                        "'" + DUI + "', " +
                        "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "';");
                Save_ID = getDatabaseValue<int>("Save_ID", "SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
                foreach (Level_Info level in gameController.Level_Data)
                {
                    d.Log("Inserting level #" + level.Number + " Name: " + level.Name + " Medals: " + level.Medals,false);
                    reader = queryDatabase("INSERT INTO tbl_Level_Save_Data " +
                            "(Map_ID, Save_ID, Medals_Earned) " +
                        "SELECT " + level.Number + ", " +
                                    Save_ID + ", " +
                                    level.Medals + ";");
                }
            }
        }
        catch (System.Exception ex)
        {
            d.Log("Database save failed: " + ex.Message + ", saving to dat file.",true);
        }
        //Save to bin file
        if (!File.Exists(Application.persistentDataPath + "/save.dat"))
            File.Create(Application.persistentDataPath + "/save.dat");
        FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            BinaryWriter bw = new BinaryWriter(file);
            bw.Write(gameController.Medals_Earned);
            bw.Write(gameController.Current_Medals);
            bw.Write(gameController.Current_Gems);
            bw.Write(gameController.Total_Gems_Earned);
            bw.Write(DateTime.Now.ToString());
            foreach (Level_Info level in gameController.Level_Data)
            {
                bf.Serialize(file, level);
            }
        }
        catch (SerializationException e)
        {
            d.Log(e.Message, true);
        }
        finally
        {
            file.Close();
        }
    }
    public void LoadData()
    {
        //Get local save.dat binary file to check Modified date
        if (File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                BinaryReader br = new BinaryReader(file);
                byte i = 0;
                byte Medals_Earned_ = br.ReadByte();
                d.Log("Loaded Medals_Earned: " + Medals_Earned_, false);
                byte Current_Medals_ = br.ReadByte();
                d.Log("Loaded Current_Medals: " + Current_Medals_, false);
                int Current_Gems_ = br.ReadInt32();
                d.Log("Loaded Current_Gems: " + Current_Gems_, false);
                int Total_Gems_Earned_ = br.ReadInt32();
                d.Log("Loaded Total_Gems_Earned: " + Total_Gems_Earned_, false);
                DateTime Modified_ = Convert.ToDateTime(br.ReadString());
                d.Log("Loaded Modified: " + Modified_, false);
                d.Log("Loaded Total_Gems_Earned: " + Total_Gems_Earned_, false);
                List<Level_Info> Level_Data_ = new List<Level_Info>();
                while (br.PeekChar() != -1)
                {
                    try
                    {
                        Level_Data_.Add((Level_Info)bf.Deserialize(file));
                    }
                    catch (SerializationException e)
                    {
                        d.Log("SerializationException: This is most likely the end of the file.", false);
                        d.Log(e.Message, false);
                        break;
                    }
                    d.Log("Loaded Level_Info: ", false);
                    d.Log("Level Name: " + Level_Data_[i].Name + " Level Number: " + Level_Data_[i].Number + " Medals Earned: " + Level_Data_[i].Medals, false);
                    ++i;
                    }
            }
            catch (SerializationException e)
            {
                d.Log(e.Message, true);
                throw;
            }
            finally
            {
                file.Close();
            }
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
    }
    private void Start()
    {
        //Testing:
        SaveData();
        LoadData();
    }
    #endregion

}

public class Debug_Log
{
    bool Allow_Logs = true;

    public void Log(string source, string output, bool toConsole)
    {
        if (!Allow_Logs) return;
        if (toConsole) Debug.Log(output);
        using (StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/debug_log.txt", true))
        {
            writer.WriteLine(source + " - " + DateTime.Now + ": " + output);
        }
    }
    public List<string> Retrieve()
    {
        if (!Allow_Logs) return null;
        List<string> temp = new List<string>();
        using (StreamReader reader = new StreamReader(Application.persistentDataPath + "/debug_log.txt"))
        {
            temp.Add(reader.ReadLine());
        }
        return temp;
    }
}

[Serializable]
public class Local_Cache
{
    private GameController gameController;
    private Debug_Log d = new Debug_Log();
    public static Dictionary<string, float> Leaderboard = new Dictionary<string, float>();
    public static Dictionary<string, float> Store = new Dictionary<string, float>();

    /// <summary>
    /// Constructor.
    /// </summary>
    public Local_Cache(GameController gameController_)
    {
        gameController = gameController_;
    }
    public void Load()
    {

    }
    /// <summary>
    /// Save game state/data to save file.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="setting">0 = Game save data, 1 = Leaderboard save data, 2 = Store save data</param>
    public void Save(MySqlDataReader reader, byte setting)
    {
        string path = getFilename(setting);
        if (path == "ERROR")
        {
            d.Log("Local_Cache.Save", "Error code returned when attempting to call the function getFilename", true);
            return;
        }
        while (reader.Read())
        {
            switch (setting)
            {
                case 0://Player_Save.dat
                    break;
                case 1://Leaderboard_Save.dat
                    Leaderboard.Clear();
                    Leaderboard.Add(reader["Player_Name"].ToString(), (float)reader["Score"]);
                    break;
                case 2://Store_Save.dat
                    Store.Clear();
                    Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
                    break;
                default:
                    break;
            }
        }
    }
    private string getFilename(byte setting)
    {
        switch (setting)
        {
            case 0:
                return "Player_Save.dat";
            case 1:
                return "Leaderboard_Save.dat";
            case 2:
                return "Store_Save.dat";
            default:
                return "ERROR";
        }
    }
}