using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;
using System.Runtime.CompilerServices;

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

    #region Save Data
    public void SaveData()
    {
        d.Log("SaveData() DB Saves Started.", true);
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
                d.Log("Database save data found, updating record.", false);
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
                        d.Log("Level save check returned null, skipping save of level #" + level.Number + ": " + level.Name + " (" + level.Medals + ")", false);
                        continue;
                    }
                    //Check whether the map ID entered exists in the map data table, if not skip
                    Map_Exists_Check = DB.checkDatabase("SELECT * FROM tbl_Map_Data WHERE Level_Number = " + level.Number + ";");
                    if (Map_Exists_Check == null || Map_Exists_Check == false)
                    {
                        d.Log("Map #" + level.Number + " does not exist, skipping save of level: " + level.Name + " (" + level.Medals + " Medals)", false);
                        continue;
                    }
                    else if (Level_Save_Check == true && Map_Exists_Check == true)
                    {
                        //Data found for item (update)
                        d.Log("Data found for level save #" + level.Number + ", updating to, Medals: " + level.Medals + " Number: " + level.Number, false);
                        reader = DB.queryDatabase("UPDATE tbl_Level_Save_Data " +
                                "SET Medals_Earned = " + level.Medals + " " +
                                "WHERE Save_ID = " + Save_ID + ";");
                    }
                    else
                    {
                        //No data found for item (insert)
                        d.Log("Data not found for level save #" + level.Number + ", inserting, Medals: " + level.Medals + " Name: " + level.Name, false);
                        reader = DB.queryDatabase("INSERT INTO tbl_Level_Save_Data" +
                                "(Map_ID, Save_ID, Medals_Earned) " +
                            "SELECT " + level.Number + ", " + Save_ID + ", " + level.Medals + ";");
                    }
                }
            }
            else
            {
                //No data
                d.Log("Database save data not found, inserting new record.", false);
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
                    d.Log("Inserting level #" + level.Number + " Name: " + level.Name + " Medals: " + level.Medals, false);
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
            d.Log("Database save failed: " + ex.Message + ", saving to dat file.",true);
        }
        d.Log("SaveData() DB Saves Finished.", true);
        d.Log("SaveData() Binary Saves Started.", true);
        LC.Save(Local_Cache.Setting.Player_Save);
        d.Log("SaveData() Binary Saves Finished.", true);
    }
    /// <summary>
    /// Load game data into local memory ready for use in-game, bools are optional with a default of true.
    /// </summary>
    /// <param name="Game_State">Do you want to load the game state? This will check the dates in the database and local binary file to load the most recent save and update the other.</param>
    /// <param name="Leaderboard">Do you want to load the Leaderboard into its Dictionary? Results are automatically sorted by Score and can be accessed using the Leaderboard function (returns Dictionary[string,float]).</param>
    /// <param name="Store">Do you want to load the Store into its Dictionary? This can be accessed using the Store function (returns Dictionary[string,float]).</param>
    public void LoadData(bool Game_State = true, bool Leaderboard = true, bool Store = true)
    {
        //Get local save.dat binary file to check Modified date
        if (!LC.Load(DB.getDatabaseValue<DateTime>("Modified", "SELECT Modified FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';"), Game_State, Leaderboard, Store))
            DB_Load();
    }
    private void DB_Load()
    {
        d.Log("LoadData().Load() DB Save Load Started.", true);
        d.Log("Saving game state from database data set..",true);
        MySqlDataReader reader;
        //Load main save data from database
        reader = DB.queryDatabase("SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
        if (reader.Read())
        {
            Save_ID = (int)reader["Save_ID"];
            GameState.Total_Medals_Earned = (byte)reader["Medals_Earned"];
            GameState.Current_Medals = (byte)reader["Current_Medals"];
            GameState.Current_Gems = (int)reader["Current_Gems"];
            GameState.Total_Gems_Earned = (int)reader["Total_Gems_Earned"];
        }
        //Load level save data from database
        reader = DB.queryDatabase("SELECT " + 
                "Medals_Earned.tbl_Level_Save_Data AS ME, " +
                "Level_Name.tbl_Map_Data AS LN, " + 
                "Map_ID.tbl_Map_Data AS MI " + 
            "FROM tbl_Level_Save_Data " + 
            "LEFT JOIN tbl_Map_Data " + 
                "ON tbl_Level_Save_Data.Map_ID = tbl_Map_Data.Map_ID " + 
            "WHERE tbl_Level_Save_Data.Save_ID = " + Save_ID + ";");
        if (reader.Read())
            GameState.Level_Data.Clear();
        while (reader.Read())
        {
            GameState.Level_Data.Add(new Level_Info(reader["LN"].ToString(), (byte)reader["MI"], (byte)reader["ME"]));
        }
        d.Log("LoadData().Load() DB Save Load Finished.", true);
        d.Log("LoadData().Load() Local Cache Save Load Started.", true);
        LC.Save(Local_Cache.Setting.Player_Save);
        d.Log("LoadData().Load() Local Cache Save Load Finished.", true);
    }
    #endregion

    #region Leaderboard
    /// <summary>
    /// Submit Score and get the updated Leaderboard.
    /// </summary>
    /// <param name="Score">Score the Player achieved. (float)</param>
    /// <param name="Player_Name">Player's name. (string)</param>
    public Dictionary<string, float> Leaderboard(float Score, string Player_Name)
    {
        MySqlDataReader reader = DB.queryDatabase("SELECT * FROM tbl_Highscores WHERE Player_Name = '" + Player_Name + "';");
        if (reader != null)
        {
            if (reader.Read())
            {
                reader = DB.queryDatabase("UPDATE tbl_Highscores SET Score = " + Score + " WHERE Player_Name = '" + Player_Name + "';");
            }
            else
            {
                reader = DB.queryDatabase("INSERT INTO tbl_Highscores (Player_Name, Score) SELECT '" + Player_Name + "', " + Score + ";");
            }
        }
        LoadData(false, true, false);
        return (LC.Leaderboard);
    }
    /// <summary>
    /// Updates and returns the Leaderboard from the local cache, this can be updated using LoadData and selecting true for the Leaderboard bool.
    /// </summary>
    /// <returns>The local Leaderboard cache.</returns>
    public Dictionary<string, float> Leaderboard()
    {
        LoadData(false, true, false);
        return (LC.Leaderboard);
    }
    #endregion

    #region Store
    /// <summary>
    /// Pulls the Store again and returns the cost of the Item passed via parameter.
    /// </summary>
    /// <param name="Item_Name">Name of the item you want to know the price for.</param>
    /// <returns>The price of the item (float).</returns>
    public float Store(string Item_Name)
    {
        LoadData(false, false, true);
        return LC.Store[Item_Name];
    }
    /// <summary>
    /// Pulls the Store again and returns the Dictionary.
    /// </summary>
    /// <returns>Dictionary[string,float]</returns>
    public Dictionary<string, float> Store()
    {
        LoadData(false, false, true);
        return (LC.Store);
    }
    #endregion

    #region General Initialisation
    private void Awake()
    {
        //Initialising the local cache in awake due to constructor
        LC = new Local_Cache(GameState, Application.persistentDataPath + "/", DB);
        DUI = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Unique Client ID: " + DUI);
    }
    #endregion

    #region Cache Debugging
    public void Print_Cache()
    {
        d.Log(LC.Cache(), true);
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
    static bool Allow_Logs = true;//Change this to false if you no longer want anything to be logged at all

    public void Log(string output, bool toConsole, [CallerLineNumber] int LineNumber = 0, [CallerMemberName] string Caller = null)
    {
        if (!Allow_Logs) return;
        if (toConsole) Debug.Log(output);
        using (StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/debug_log.txt", true))
        {
            writer.WriteLine(DateTime.Now + " // Caller: " + Caller + ", Line Number: " + LineNumber + ": " + output);
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
    private Database_Interaction DB;
    private GameController gameController;
    private Debug_Log d = new Debug_Log();
    public Dictionary<string, float> Leaderboard = new Dictionary<string, float>();
    public Dictionary<string, float> Store = new Dictionary<string, float>();

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
    public Local_Cache(Game_State GS_, string Path_, Database_Interaction DB_)
    {
        GS = GS_;
        Path = Path_;
        DB = DB_;
    }
    /// <summary>
    /// Loads: Player_Save.dat
    /// Checks against databases modified date and loads the most recent.
    /// </summary>
    public bool Load(DateTime DB_DT, bool Game_State_, bool Leaderboard_, bool Store_)
    {
        d.Log("LoadData().Load() DB Leaderboard Load Started.", true);
        //Loading the leaderboard and store first to prevent early return.
        MySqlDataReader reader;
        if (Leaderboard_)
        {
            reader = DB.queryDatabase("SELECT * FROM tbl_Highscores ORDER BY Score ASC;");
            if (reader == null)
                 return false; 
            Leaderboard.Clear();
            while (reader.Read())
            {
                Leaderboard.Add(reader["Player_Name"].ToString(), (float)reader["Score"]);
                d.Log("Read Leaderboard item: Player Name: " + reader["Player_Name"].ToString() + " , Score: " + (float)reader["Score"], true);
            }
        }
        d.Log("LoadData().Load() DB Leaderboard Load Finished.", true);
        d.Log("LoadData().Load() DB Store Load Started.", true);
        if (Store_)
        {
            reader = DB.queryDatabase("SELECT * FROM tbl_Store WHERE In_Use = 1;");
            if (reader.Read())
            {
                Store.Clear();
                while (reader.Read())
                {
                    Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
                    d.Log("Read Store item: Item Name: " + reader["Item_Name"].ToString() + " , Item_Price: " + (float)reader["Item_Price"], true);
                }
            }
        }
        d.Log("LoadData().Load() DB Store Load Finished.", true);
        //This shouldn't be used for the leaderboard or store due to the dynamic nature of the data set.
        d.Log("LoadData().Load() Binary Save Load Started.", true);
        if (File.Exists(Path + File_Names[(int)Setting.Player_Save]) && Game_State_) 
        {
            int Total_Gems_Earned_;
            int Current_Gems_;
            byte Current_Medals_;
            byte Total_Medals_Earned_;
            DateTime Modified_;
            List<Level_Info> Level_Data_ = new List<Level_Info>();
            FileStream file = File.Open(Path + File_Names[(int)Setting.Player_Save], FileMode.Open);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                BinaryReader br = new BinaryReader(file);
                byte i = 0;
                Total_Medals_Earned_ = br.ReadByte();
                d.Log("Loaded Medals_Earned: " + Total_Medals_Earned_, false);
                Current_Medals_ = br.ReadByte();
                d.Log("Loaded Current_Medals: " + Current_Medals_, false);
                Current_Gems_ = br.ReadInt32();
                d.Log("Loaded Current_Gems: " + Current_Gems_, false);
                Total_Gems_Earned_ = br.ReadInt32();
                d.Log("Loaded Total_Gems_Earned: " + Total_Gems_Earned_, false);
                Modified_ = Convert.ToDateTime(br.ReadString());
                d.Log("Loaded Modified: " + Modified_, false);
                if (DB_DT < Modified_)
                {
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
            }
            catch (SerializationException e)
            {
                d.Log(e.Message, true);
                return false;
            }
            finally
            {
                file.Close();
            }
            if (DB_DT < Modified_)
            {
                d.Log("The .bin files modified date is later than the databases, loading the local file into memory", true);
                GS.Current_Gems = Current_Gems_;
                GS.Total_Gems_Earned = Total_Gems_Earned_;
                GS.Current_Medals = Current_Medals_;
                GS.Total_Medals_Earned = Total_Medals_Earned_;
                GS.Level_Data = Level_Data_;
                d.Log("LoadData().Load() Binary Save Load Finished.", true);
                return true;
            }
            else
            {
                d.Log("LoadData().Load() Binary Save Load Finished.", true);
                return false;
            }
        }
        else
        {
            d.Log("LoadData().Load() Binary Save Load Finished.", true);
            return false;
        }
    }
    /// <summary>
    /// Saves the current game state from gameController object for player save or database for leaderboard/store data.
    /// </summary>
    /// <param name="setting">Local_Cache.Setting.</param>
    /// <param name="reader">The MySqlDataReader recieved from the database. [OPTIONAL]</param>
    public void Save(Setting setting, MySqlDataReader reader = null)
    {
        if (setting == Setting.Player_Save)
        {
            FileStream file;
            if (!File.Exists(Path + File_Names[(int)Setting.Player_Save]))
                file = File.Create(Path + File_Names[(int)Setting.Player_Save]);
            else
                file = File.Open(Path + File_Names[(int)Setting.Player_Save], FileMode.Open);

            try
            {
                d.Log("Creating save file with the following data:", false);
                BinaryFormatter bf = new BinaryFormatter();
                BinaryWriter bw = new BinaryWriter(file);
                bw.Write(GS.Total_Medals_Earned);
                bw.Write(GS.Current_Medals);
                bw.Write(GS.Current_Gems);
                bw.Write(GS.Total_Gems_Earned);
                bw.Write(DateTime.Now.ToString());
                d.Log("Total_Medals_Earned: " + GS.Total_Gems_Earned + ", Current_Medals: " + GS.Current_Medals + ", Current_Gems: " + GS.Current_Gems + ", Total_Gems_Earned: " + GS.Total_Gems_Earned + ", Modified: " + DateTime.Now.ToString(), true);
                foreach (Level_Info level in GS.Level_Data)
                {
                    bf.Serialize(file, level);
                    d.Log("   > Level #" + level.Number + ": " + level.Name + " (" + level.Medals + " Medals Earned)", true);
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
        else
        {
            while (reader.Read())
            {
                switch (setting)
                {

                    case Setting.Leadboard_Save:
                        {
                            d.Log("Saving leaderboard data..", false);
                            Leaderboard.Clear();
                            Leaderboard.Add(reader["Player_Name"].ToString(), (float)reader["Score"]);
                            break;
                        }
                    case Setting.Store_Save:
                        {
                            d.Log("Saving store data..", false);
                            Store.Clear();
                            Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
                            break;
                        }
                    default:
                        {
                            d.Log("Incorrect setting passed through.", true);
                            break;
                        }
                }
            }
        }
    }
    public string Cache()
    {
        string temp = "";
        byte i = 1;
        temp = "GAME_STATE_CACHE >> Current Medals: " + GS.Current_Medals + ", Total Medals: " + GS.Total_Medals_Earned + ", Current Gems: " + GS.Current_Gems + ", Total Gems: " + GS.Total_Medals_Earned + ", Modified: " + GS.Modified;
        foreach (Level_Info level in GS.Level_Data)
        {
            temp += ", " + "Level #" + i + " - Name: " + level.Name + ", Number: " + level.Number + ", Medals: " + level.Medals;
            ++i;
        }
        return temp;
    }
}
[Serializable]
public class Game_State
{
    // This is the time of the last game state update.
    private DateTime modified = new DateTime();
    public DateTime Modified { get { return this.modified; } }

    // The amount of Medals the Player currently has.
    private byte current_Medals = 0;
    public byte Current_Medals { get { return this.current_Medals; } set { this.current_Medals = value; modified = DateTime.Now; } }

    // The total amount of Medals the Player has earned.
    //This is probably not necessary but I wanted to put it in just in case we wanted to give the Player the ability to trade them.
    private byte total_Medals_Earned = 0;
    public byte Total_Medals_Earned { get { return this.total_Medals_Earned; } set { this.total_Medals_Earned = value; modified = DateTime.Now; } }

    // The amount of Gems currently in the Players inventory.
    private int current_Gems = 0;
    public int Current_Gems { get { return this.current_Gems; } set { this.current_Gems = value; modified = DateTime.Now; } }

    // The lifetime total amount of Gems the Player has earned.
    private int total_Gems_Earned = 0;
    public int Total_Gems_Earned { get { return this.total_Gems_Earned; } set { this.total_Gems_Earned = value; modified = DateTime.Now; } }

    // List of all level saves completed (Not used for non-completed levels).
    public List<Level_Info> Level_Data = new List<Level_Info>();
}
[Serializable]
public class Level_Info
{
    // The name of the level being saved.
    private string name = "";
    public string Name { get { return name; } }

    // The number of the level being saved.
    private byte number = 0;
    public byte Number { get { return number; } }

    // The amount of medals earned by the player for this level.
    private byte medals = 1;
    public byte Medals { get { return medals; } }

    /// <summary>
    /// Constructor for Level_Info
    /// </summary>
    /// <param name="Name_">The name of the level being saved</param>
    /// <param name="Number_">The number of the level being saved</param>
    /// <param name="Medals_">The number of medals being saved (1 to 3)</param>
    public Level_Info(string Name_, byte Number_, byte Medals_)
    {
        name = Name_;
        number = Number_;
        medals = Medals_;
    }
}