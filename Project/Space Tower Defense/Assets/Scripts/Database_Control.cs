using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;

public class Database_Control : MonoBehaviour {

    public Game_State GameState = new Game_State();
    
    private Debug_Log d = new Debug_Log();
    private Local_Cache LC;
    private Database_Interaction DB = new Database_Interaction();

    #region Save State Variables
    private string DUI = "";
    [HideInInspector]
    public int Save_ID = 0;
    private byte Map_ID = 0;
    private string Map_Name = "";
    #endregion

    #region Store Data
    /// <summary>
    /// Refreshes the data dictionaries data with the current store items and prices.
    /// Items are sorted by their auto-generated ID.
    /// </summary>
    public void RefreshStore()
    {
        MySqlDataReader reader = DB.queryDatabase("SELECT Item_ID, Item_Name, Item_Price FROM tbl_Store WHERE In_Use = 1 ORDER BY Item_ID ASC;");
        if (reader == null)
            d.Log("Database.RefreshStoreData","RefreshStoreData query didn't return a value.",true);
        else
            LC.Save(reader, Local_Cache.Setting.Store_Save);
    }
    /// <summary>
    /// Get an items current store price by its name (Returns -1 if nothing was found).
    /// </summary>
    /// <param name="Item_Name">The name of the item you want the price for.</param>
    /// <returns>The price of the item passed via function parameters.</returns>
    public float GetItemPrice(string Item_Name)
    {
        MySqlDataReader reader = null;
        reader = DB.queryDatabase("SELECT Item_Price FROM tbl_Store WHERE Item_Name = '" + Item_Name + "' AND In_Use = 1;");
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
    public void RefreshLeaderboard()
    {
        MySqlDataReader reader = DB.queryDatabase("SELECT Player_Name, Score FROM tbl_Highscores ORDER BY Score ASC;");
        if (reader == null)
            d.Log("Database.RefreshHighscoresData","RefreshHighscoresData didn't return a value.",true);
        else
            LC.Save(reader, Local_Cache.Setting.Leadboard_Save);
    }
        #region SubmitScore
        /// <summary>
        /// Submit a highscore to the Highscores database table.
        /// Each player gets 1 record each, if the score submitted is higher than the currently logged one it is overwritten.
        /// </summary>
        /// <param name="Player_Name">Name of the player who submitted the score.</param>
        /// <param name="Score">The score achieved and wanting to be logged.</param>
        public void SubmitScore<T>(string Player_Name, T Score)
        {
            MySqlDataReader reader = null;
            reader = DB.queryDatabase("SELECT * FROM tbl_Highscores WHERE Player_Name = '" + Player_Name + "';");
            if (reader.Read())
            {
                if ((float)(object)Score > (float)reader["Score"])
                {
                    reader = DB.queryDatabase("UPDATE tbl_Highscores SET Score = " + Score + " WHERE Player_Name = '" + Player_Name + "';");
                }
            }
            else
            {
                reader = DB.queryDatabase("INSERT INTO tbl_Highscores (Player_Name, Score) SELECT '" + Player_Name + "', " + Score + ";");
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
            Save_ID = DB.getDatabaseValue<int>("Save_ID", "SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
            bool? Level_Save_Check;
            bool? Map_Exists_Check;
            if (Save_ID > 0)
            {
                d.Log("Database.SaveData", "Database save data found, updating record.", false);
                reader = DB.queryDatabase("UPDATE tbl_Save_Data " +
                    "SET Medals_Earned = " + GameState.Total_Medals_Earned + ", " +
                        "Current_Medals = " + GameState.Current_Medals + ", " +
                        "Current_Gems = " + GameState.Current_Gems + ", " +
                        "Total_Gems_Earned = " + GameState.Total_Gems_Earned + ", " +
                        "Modified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                    "WHERE Unique_Identifier = '" + DUI + "';");
                foreach (Level_Info level in GameState.Level_Data)
                {
                    //Check for save data for current level
                    Level_Save_Check = DB.checkDatabase("SELECT * FROM tbl_Level_Save_Data " +
                        "LEFT JOIN tbl_Map_Data " +
                            "ON tbl_Map_Data.Level_Number = tbl_Level_Save_Data.Map_ID " +
                        "WHERE Save_ID = " + Save_ID + " AND " +
                            "(tbl_Map_Data.Level_Name = '" + level.Name + "' OR tbl_Map_Data.Level_Number = " + level.Number + ");");
                    if (Level_Save_Check == null)
                    {
                        d.Log("Database.SaveData", "Level save check returned null, skipping save of level #" + level.Number + ": " + level.Name + " (" + level.Medals + ")", false);
                        continue;
                    }
                    //Check whether the map ID entered exists in the map data table, if not skip
                    Map_Exists_Check = DB.checkDatabase("SELECT * FROM tbl_Map_Data WHERE Level_Number = " + level.Number + ";");
                    if (Map_Exists_Check == null || Map_Exists_Check == false)
                    {
                        d.Log("Database.SaveData", "Map #" + level.Number + " does not exist, skipping save of level: " + level.Name + " (" + level.Medals + " Medals)", false);
                        continue;
                    }
                    else if (Level_Save_Check == true && Map_Exists_Check == true)
                    {
                        //Data found for item (update)
                        d.Log("Database.SaveData", "Data found for level save #" + level.Number + " Medals: " + level.Medals + " Number: " + level.Number, false);
                        reader = DB.queryDatabase("UPDATE tbl_Level_Save_Data " +
                                "SET Medals_Earned = " + level.Medals + " " +
                                "WHERE Save_ID = " + Save_ID + ";");
                    }
                    else
                    {
                        //No data found for item (insert)
                        d.Log("Database.SaveData", "Data not found for level save #" + level.Number + " Medals: " + level.Medals + " Name: " + level.Name, false);
                        reader = DB.queryDatabase("INSERT INTO tbl_Level_Save_Data" +
                                "(Map_ID, Save_ID, Medals_Earned) " +
                            "SELECT " + level.Number + ", " + Save_ID + ", " + level.Medals + ";");
                    }
                }
            }
            else
            {
                //No data
                d.Log("Database.SaveData", "Database save data not found, inserting new record.", false);
                reader = DB.queryDatabase("INSERT INTO tbl_Save_Data " +
                        "(Medals_Earned, Current_Medals, Current_Gems, Total_Gems_Earned, Unique_Identifier, Modified) " +
                    "SELECT " + GameState.Total_Medals_Earned + ", " +
                        GameState.Current_Medals + ", " +
                        GameState.Current_Gems + ", " +
                        GameState.Total_Gems_Earned + ", " +
                        "'" + DUI + "', " +
                        "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "';");
                Save_ID = DB.getDatabaseValue<int>("Save_ID", "SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
                foreach (Level_Info level in GameState.Level_Data)
                {
                    d.Log("Database.SaveData", "Inserting level #" + level.Number + " Name: " + level.Name + " Medals: " + level.Medals, false);
                    reader = DB.queryDatabase("INSERT INTO tbl_Level_Save_Data " +
                            "(Map_ID, Save_ID, Medals_Earned) " +
                        "SELECT " + level.Number + ", " +
                                    Save_ID + ", " +
                                    level.Medals + ";");
                }
            }
        }
        catch (System.Exception ex)
        {
            d.Log("Database.SaveData", "Database save failed: " + ex.Message + ", saving to dat file.",true);
        }
    }
    public void LoadData()
    {
        //Get local save.dat binary file to check Modified date
        LC.Load();
    }
    #endregion

    #region General Initialisation
    private void Awake()
    {
        //Initialising the local cache in awake due to constructor
        LC = new Local_Cache(GameState, Application.persistentDataPath + "/");
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

public class Database_Interaction
{
    #region Database Variables
    private MySqlConnection connection = null;
    private MySqlCommand command = null;
    private string connectionString = "Server = den1.mysql5.gear.host; port = 3306; Database = stddb; User = stdclient; Password = '8ch8J5PPRRCFKp6!';";
    private float defaultRefreshTime = 5.0f; //Amount of seconds between database refreshes
    #endregion

    #region Query Database
    public MySqlDataReader queryDatabase(string query)
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
    public bool? checkDatabase(string query)
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
    public T getDatabaseValue<T>(string value, string query)
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
    private Game_State GS;
    private GameController gameController;
    private Debug_Log d = new Debug_Log();
    public static Dictionary<string, float> Leaderboard = new Dictionary<string, float>();
    public static Dictionary<string, float> Store = new Dictionary<string, float>();

    private string Path = "";
    private readonly List<string> File_Names = new List<string>() { "Player_Save.dat", "Leaderboard_Save.dat", "Store_Save.dat" };

    public enum Setting
    {
        Player_Save,
        Leadboard_Save,
        Store_Save
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public Local_Cache(Game_State GS_, string Path_)
    {
        GS = GS_;
        Path = Path_;
    }
    /// <summary>
    /// Loads: Player_Save.dat
    /// Checks against databases modified date and loads the most recent.
    /// </summary>
    public void Load()
    {
        //This shouldn't be used for the leaderboard or store due to the dynamic nature of the data set.
        if (File.Exists(Path + File_Names[(int)Setting.Player_Save])) 
        {
            FileStream file = File.Open(Path + File_Names[(int)Setting.Player_Save], FileMode.Open);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                BinaryReader br = new BinaryReader(file);
                byte i = 0;
                byte Medals_Earned_ = br.ReadByte();
                d.Log("Local_Cache.Load", "Loaded Medals_Earned: " + Medals_Earned_, false);
                byte Current_Medals_ = br.ReadByte();
                d.Log("Local_Cache.Load", "Loaded Current_Medals: " + Current_Medals_, false);
                int Current_Gems_ = br.ReadInt32();
                d.Log("Local_Cache.Load", "Loaded Current_Gems: " + Current_Gems_, false);
                int Total_Gems_Earned_ = br.ReadInt32();
                d.Log("Local_Cache.Load", "Loaded Total_Gems_Earned: " + Total_Gems_Earned_, false);
                DateTime Modified_ = Convert.ToDateTime(br.ReadString());
                d.Log("Local_Cache.Load", "Loaded Modified: " + Modified_, false);
                d.Log("Local_Cache.Load", "Loaded Total_Gems_Earned: " + Total_Gems_Earned_, false);
                List<Level_Info> Level_Data_ = new List<Level_Info>();
                while (br.PeekChar() != -1)
                {
                    try
                    {
                        Level_Data_.Add((Level_Info)bf.Deserialize(file));
                    }
                    catch (SerializationException e)
                    {
                        d.Log("Local_Cache.Load", "SerializationException: This is most likely the end of the file.", false);
                        d.Log("Local_Cache.Load", e.Message, false);
                        break;
                    }
                    d.Log("Local_Cache.Load", "Loaded Level_Info: ", false);
                    d.Log("Local_Cache.Load", "Level Name: " + Level_Data_[i].Name + " Level Number: " + Level_Data_[i].Number + " Medals Earned: " + Level_Data_[i].Medals, false);
                    ++i;
                }
            }
            catch (SerializationException e)
            {
                d.Log("Local_Cache.Load", e.Message, true);
                throw;
            }
            finally
            {
                file.Close();
            }
        }
        else
	    {
            d.Log("Local_Cache.Load", "The .dat file for Player_Save.dat does not exist in the expected local directory.", true);
        }
    }
    
    /// <summary>
    /// Saves the current game state from gameController object for player save or database for leaderboard/store data.
    /// </summary>
    /// <param name="reader">The MySqlDataReader recieved from the database.</param>
    /// <param name="setting">Local_Cache.Setting.</param>
    public void Save(MySqlDataReader reader, Setting setting)
    {
        //Read data passed through via db connection
        while (reader.Read())
        {
            switch (setting)
            {
                case Setting.Player_Save:
                    {
                        if (!File.Exists(Path + File_Names[(int)Setting.Player_Save]))
                            File.Create(Path + File_Names[(int)Setting.Player_Save]);
                        FileStream file = File.Open(Path + File_Names[(int)Setting.Player_Save], FileMode.Open);
                        try
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            BinaryWriter bw = new BinaryWriter(file);
                            bw.Write(GS.Total_Medals_Earned);
                            bw.Write(GS.Current_Medals);
                            bw.Write(GS.Current_Gems);
                            bw.Write(GS.Total_Gems_Earned);
                            bw.Write(DateTime.Now.ToString());
                            foreach (Level_Info level in GS.Level_Data)
                            {
                                bf.Serialize(file, level);
                            }
                        }
                        catch (SerializationException e)
                        {
                            d.Log("Local_Cache.SaveData", e.Message, true);
                        }
                        finally
                        {
                            file.Close();
                        }
                        break;
                    }
                case Setting.Leadboard_Save:
                    {
                        d.Log("Local_Cache", "Saving leaderboard data..", false);
                        Leaderboard.Clear();
                        Leaderboard.Add(reader["Player_Name"].ToString(), (float)reader["Score"]);
                        break;
                    }
                case Setting.Store_Save:
                    {
                        d.Log("Local_Cache", "Saving store data..", false);
                        Store.Clear();
                        Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
                        break;
                    }
                default:
                    {
                        d.Log("Local_Cache", "Incorrect setting passed through.", true);
                        break;
                    }
            }
        }
    }
}
[Serializable]
public class Game_State
{
    public DateTime Modified = new DateTime();
    public byte Current_Medals = 0;
    public byte Total_Medals_Earned = 0;
    public int Current_Gems = 0;
    public int Total_Gems_Earned = 0;
    public List<Level_Info> Level_Data = new List<Level_Info>();
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