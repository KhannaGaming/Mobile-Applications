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
    public Leaderboard leaderboard = new Leaderboard();
    public Store store = new Store();

    private Debug_Log d = new Debug_Log();

    #region Save State Variables
    private string DUI = "";
    [HideInInspector]
    public int Save_ID = 0;
    #endregion    

    #region Save Data Depreciated
    //public void SaveData()
    //{
    //    d.Log("SaveData() DB Saves Started.", true);
    //    if (DUI == "") DUI = SystemInfo.deviceUniqueIdentifier;
    //    Save to database
    //    try
    //    {
    //        Check for existing save data in database:
    //        MySqlDataReader reader;
    //        Save_ID = getDatabaseValue<int>("Save_ID", "SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
    //        bool? Level_Save_Check;
    //        bool? Map_Exists_Check;
    //        if (Save_ID > 0)
    //        {
    //            d.Log("Database save data found, updating record.", false);
    //            reader = queryDatabase("UPDATE tbl_Save_Data " +
    //                "SET Medals_Earned = " + GameState.Total_Medals_Earned + ", " +
    //                    "Current_Medals = " + GameState.Current_Medals + ", " +
    //                    "Current_Gems = " + GameState.Current_Gems + ", " +
    //                    "Total_Gems_Earned = " + GameState.Total_Gems_Earned + ", " +
    //                    "Modified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
    //                "WHERE Unique_Identifier = '" + DUI + "';");
    //            foreach (Level_Info level in GameState.Level_Data)
    //            {
    //                Check for save data for current level
    //                Level_Save_Check = DB.checkDatabase("SELECT * FROM tbl_Level_Save_Data " +
    //                    "LEFT JOIN tbl_Map_Data " +
    //                        "ON tbl_Map_Data.Level_Number = tbl_Level_Save_Data.Map_ID " +
    //                    "WHERE Save_ID = " + Save_ID + " AND " +
    //                        "(tbl_Map_Data.Level_Name = '" + level.Name + "' OR tbl_Map_Data.Level_Number = " + level.Number + ");");
    //                if (Level_Save_Check == null)
    //                {
    //                    d.Log("Level save check returned null, skipping save of level #" + level.Number + ": " + level.Name + " (" + level.Medals + ")", false);
    //                    continue;
    //                }
    //                Check whether the map ID entered exists in the map data table, if not skip
    //                Map_Exists_Check = DB.checkDatabase("SELECT * FROM tbl_Map_Data WHERE Level_Number = " + level.Number + ";");
    //                if (Map_Exists_Check == null || Map_Exists_Check == false)
    //                {
    //                    d.Log("Map #" + level.Number + " does not exist, skipping save of level: " + level.Name + " (" + level.Medals + " Medals)", false);
    //                    continue;
    //                }
    //                else if (Level_Save_Check == true && Map_Exists_Check == true)
    //                {
    //                    Data found for item (update)
    //                    d.Log("Data found for level save #" + level.Number + ", updating to, Medals: " + level.Medals + " Number: " + level.Number, false);
    //                    reader = DB.queryDatabase("UPDATE tbl_Level_Save_Data " +
    //                            "SET Medals_Earned = " + level.Medals + " " +
    //                            "WHERE Save_ID = " + Save_ID + ";");
    //                }
    //                else
    //                {
    //                    No data found for item (insert)
    //                    d.Log("Data not found for level save #" + level.Number + ", inserting, Medals: " + level.Medals + " Name: " + level.Name, false);
    //                    reader = DB.queryDatabase("INSERT INTO tbl_Level_Save_Data" +
    //                            "(Map_ID, Save_ID, Medals_Earned) " +
    //                        "SELECT " + level.Number + ", " + Save_ID + ", " + level.Medals + ";");
    //                }
    //            }
    //        }
    //        else
    //        {
    //            No data
    //            d.Log("Database save data not found, inserting new record.", false);
    //            reader = DB.queryDatabase("INSERT INTO tbl_Save_Data " +
    //                    "(Medals_Earned, Current_Medals, Current_Gems, Total_Gems_Earned, Unique_Identifier, Modified) " +
    //                "SELECT " + GameState.Total_Medals_Earned + ", " +
    //                    GameState.Current_Medals + ", " +
    //                    GameState.Current_Gems + ", " +
    //                    GameState.Total_Gems_Earned + ", " +
    //                    "'" + DUI + "', " +
    //                    "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "';");
    //            Save_ID = DB.getDatabaseValue<int>("Save_ID", "SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
    //            foreach (Level_Info level in GameState.Level_Data)
    //            {
    //                d.Log("Inserting level #" + level.Number + " Name: " + level.Name + " Medals: " + level.Medals, false);
    //                reader = DB.queryDatabase("INSERT INTO tbl_Level_Save_Data " +
    //                        "(Map_ID, Save_ID, Medals_Earned) " +
    //                    "SELECT " + level.Number + ", " +
    //                                Save_ID + ", " +
    //                                level.Medals + ";");
    //            }
    //        }
    //    }
    //    catch (System.Exception ex)
    //    {
    //        d.Log("Database save failed: " + ex.Message + ", saving to dat file.",true);
    //    }
    //    d.Log("SaveData() DB Saves Finished.", true);
    //    d.Log("SaveData() Binary Saves Started.", true);
    //    LC.Save(Local_Cache.Setting.Player_Save);
    //    d.Log("SaveData() Binary Saves Finished.", true);
    //}
    /// <summary>
    /// Load game data into local memory ready for use in-game, bools are optional with a default of true.
    /// </summary>
    /// <param name="Game_State">Do you want to load the game state? This will check the dates in the database and local binary file to load the most recent save and update the other.</param>
    /// <param name="Leaderboard">Do you want to load the Leaderboard into its Dictionary? Results are automatically sorted by Score and can be accessed using the Leaderboard function (returns Dictionary[string,float]).</param>
    /// <param name="Store">Do you want to load the Store into its Dictionary? This can be accessed using the Store function (returns Dictionary[string,float]).</param>
    //public void LoadData(bool Game_State = true, bool Leaderboard = true, bool Store = true)
    //{
    //    Get local save.dat binary file to check Modified date
    //    if (!LC.Load(DB.getDatabaseValue<DateTime>("Modified", "SELECT Modified FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';"), Game_State, Leaderboard, Store))
    //        DB_Load();
    //}
    //private void DB_Load()
    //{
    //    d.Log("LoadData().Load() DB Save Load Started.", true);
    //    MySqlDataReader reader;
    //    Load main save data from database
    //    reader = DB.queryDatabase("SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + DUI + "';");
    //    if (reader == null)
    //        return;
    //    if (reader.Read())
    //    {
    //        Save_ID = (int)reader["Save_ID"];
    //        GameState.Total_Medals_Earned = (byte)reader["Medals_Earned"];
    //        GameState.Current_Medals = (byte)reader["Current_Medals"];
    //        GameState.Current_Gems = (int)reader["Current_Gems"];
    //        GameState.Total_Gems_Earned = (int)reader["Total_Gems_Earned"];
    //    }
    //    Load level save data from database
    //    reader = DB.queryDatabase("SELECT " + 
    //            "Medals_Earned.tbl_Level_Save_Data AS ME, " +
    //            "Level_Name.tbl_Map_Data AS LN, " + 
    //            "Map_ID.tbl_Map_Data AS MI " + 
    //        "FROM tbl_Level_Save_Data " + 
    //        "LEFT JOIN tbl_Map_Data " + 
    //            "ON tbl_Level_Save_Data.Map_ID = tbl_Map_Data.Map_ID " + 
    //        "WHERE tbl_Level_Save_Data.Save_ID = " + Save_ID + ";");
    //    if (reader == null)
    //        return;
    //    if (reader.Read())
    //        GameState.Level_Data.Clear();
    //    while (reader.Read())
    //    {
    //        GameState.Level_Data.Add(new Level_Info(reader["LN"].ToString(), (byte)reader["MI"], (byte)reader["ME"]));
    //    }
    //    d.Log("LoadData().Load() DB Save Load Finished.", true);
    //    d.Log("LoadData().Load() Local Cache Save Load Started.", true);
    //    LC.Save(Local_Cache.Setting.Player_Save);
    //    d.Log("LoadData().Load() Local Cache Save Load Finished.", true);
    //}
    #endregion

    #region General Initialisation
    private void Awake()
    {
        //Initialising the local cache in awake due to constructor
        DUI = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Unique Client ID: " + DUI);
        store.Save();
        Path = Application.persistentDataPath + "/";
    }
    #endregion
}

public class Database_Interaction
{
    public Debug_Log d = new Debug_Log();

    #region Database Variables
    private MySqlConnection connection = null;
    private MySqlCommand command = null;
    private string connectionString = "Server = den1.mysql5.gear.host; port = 3306; Database = stddb; User = stdclient; Password = '8ch8J5PPRRCFKp6!';";
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
public class Local_Cache : Database_Interaction
{
    private GameController gameController;

    internal string Path = "";
    internal string File_Name = "";

    internal FileStream file;

    // This is the time of the last game state update currently held in the games memory.
    internal DateTime Modified = new DateTime();

    public virtual void Save()
    {
        // Open binary file if necessary.
        if (file == null)
            Open_Binary_File();
        // Save data to binary file.
        // ( Overidden by derived class )
        #region Old Save Code
        //if (setting == Setting.Player_Save)
        //{
        //    FileStream file;
        //    if (!File.Exists(Path + File_Names[(int)Setting.Player_Save]))
        //        file = File.Create(Path + File_Names[(int)Setting.Player_Save]);
        //    else
        //        file = File.Open(Path + File_Names[(int)Setting.Player_Save], FileMode.Open);

        //    try
        //    {
        //        d.Log("Creating save file with the following data:", false);
        //        BinaryFormatter bf = new BinaryFormatter();
        //        BinaryWriter bw = new BinaryWriter(file);
        //        bw.Write(GS.Total_Medals_Earned);
        //        bw.Write(GS.Current_Medals);
        //        bw.Write(GS.Current_Gems);
        //        bw.Write(GS.Total_Gems_Earned);
        //        bw.Write(DateTime.Now.ToString());
        //        d.Log("Total_Medals_Earned: " + GS.Total_Gems_Earned + ", Current_Medals: " + GS.Current_Medals + ", Current_Gems: " + GS.Current_Gems + ", Total_Gems_Earned: " + GS.Total_Gems_Earned + ", Modified: " + DateTime.Now.ToString(), true);
        //        foreach (Level_Info level in GS.Level_Data)
        //        {
        //            bf.Serialize(file, level);
        //            d.Log("   > Level #" + level.Number + ": " + level.Name + " (" + level.Medals + " Medals Earned)", true);
        //        }
        //    }
        //    catch (SerializationException e)
        //    {
        //        d.Log(e.Message, true);
        //    }
        //    finally
        //    {
        //        file.Close();
        //    }
        //}
        //else
        //{
        //    while (reader.Read())
        //    {
        //        switch (setting)
        //        {

        //            case Setting.Leadboard_Save:
        //                {
        //                    d.Log("Saving leaderboard data..", false);
        //                    Leaderboard.Clear();
        //                    Leaderboard.Add(reader["Player_Name"].ToString(), (float)reader["Score"]);
        //                    break;
        //                }
        //            case Setting.Store_Save:
        //                {
        //                    d.Log("Saving store data..", false);
        //                    Store.Clear();
        //                    Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
        //                    break;
        //                }
        //            default:
        //                {
        //                    d.Log("Incorrect setting passed through.", true);
        //                    break;
        //                }
        //        }
        //    }
        //}
        #endregion
    }
    /// <summary>
    /// Load data from local cache unless it's older than the database.
    /// </summary>
    public virtual void Load()
    {
        // Open binary file if necessary
        if (file == null)
            Open_Binary_File();
        // Load from either the database or binary file (whichever is the most up to date).
        // ( Overidden by derived class )
        #region Old Load Code
        //d.Log("LoadData().Load() DB Leaderboard Load Started.", true);
        ////Loading the leaderboard and store first to prevent early return.
        //MySqlDataReader reader;
        //if (Leaderboard_)
        //{
        //    reader = DB.queryDatabase("SELECT * FROM tbl_Highscores ORDER BY Score ASC;");
        //    Leaderboard.Clear();
        //    while (reader.Read())
        //    {
        //        Leaderboard.Add(reader["Player_Name"].ToString(), (float)reader["Score"]);
        //        d.Log("Read Leaderboard item: Player Name: " + reader["Player_Name"].ToString() + " , Score: " + (float)reader["Score"], true);
        //    }
        //}
        //d.Log("LoadData().Load() DB Leaderboard Load Finished.", true);
        //d.Log("LoadData().Load() DB Store Load Started.", true);
        //if (Store_)
        //{
        //    reader = DB.queryDatabase("SELECT * FROM tbl_Store WHERE In_Use = 1;");
        //    if (reader.Read())
        //    {
        //        Store.Clear();
        //        while (reader.Read())
        //        {
        //            Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
        //            d.Log("Read Store item: Item Name: " + reader["Item_Name"].ToString() + " , Item_Price: " + (float)reader["Item_Price"], true);
        //        }
        //    }
        //}
        //d.Log("LoadData().Load() DB Store Load Finished.", true);
        ////This shouldn't be used for the leaderboard or store due to the dynamic nature of the data set.
        //d.Log("LoadData().Load() Binary Save Load Started.", true);
        //if (File.Exists(Path + File_Names[(int)Setting.Player_Save]) && Game_State_) 
        //{
        //    int Total_Gems_Earned_;
        //    int Current_Gems_;
        //    byte Current_Medals_;
        //    byte Total_Medals_Earned_;
        //    DateTime Modified_;
        //    List<Level_Info> Level_Data_ = new List<Level_Info>();
        //    FileStream file = File.Open(Path + File_Names[(int)Setting.Player_Save], FileMode.Open);
        //    try
        //    {
        //        BinaryFormatter bf = new BinaryFormatter();
        //        BinaryReader br = new BinaryReader(file);
        //        byte i = 0;
        //        Total_Medals_Earned_ = br.ReadByte();
        //        d.Log("Loaded Medals_Earned: " + Total_Medals_Earned_, false);
        //        Current_Medals_ = br.ReadByte();
        //        d.Log("Loaded Current_Medals: " + Current_Medals_, false);
        //        Current_Gems_ = br.ReadInt32();
        //        d.Log("Loaded Current_Gems: " + Current_Gems_, false);
        //        Total_Gems_Earned_ = br.ReadInt32();
        //        d.Log("Loaded Total_Gems_Earned: " + Total_Gems_Earned_, false);
        //        Modified_ = Convert.ToDateTime(br.ReadString());
        //        d.Log("Loaded Modified: " + Modified_, false);
        //        if (DB_DT < Modified_)
        //        {
        //            while (br.PeekChar() != -1)
        //            {
        //                try
        //                {
        //                    Level_Data_.Add((Level_Info)bf.Deserialize(file));
        //                }
        //                catch (SerializationException e)
        //                {
        //                    d.Log("SerializationException: This is most likely the end of the file.", false);
        //                    d.Log(e.Message, false);
        //                    break;
        //                }
        //                d.Log("Loaded Level_Info: ", false);
        //                d.Log("Level Name: " + Level_Data_[i].Name + " Level Number: " + Level_Data_[i].Number + " Medals Earned: " + Level_Data_[i].Medals, false);
        //                ++i;
        //            }
        //        }
        //    }
        //    catch (SerializationException e)
        //    {
        //        d.Log(e.Message, true);
        //        return false;
        //    }
        //    finally
        //    {
        //        file.Close();
        //    }
        //    if (DB_DT < Modified_)
        //    {
        //        d.Log("The .bin files modified date is later than the databases, loading the local file into memory", true);
        //        GS.Current_Gems = Current_Gems_;
        //        GS.Total_Gems_Earned = Total_Gems_Earned_;
        //        GS.Current_Medals = Current_Medals_;
        //        GS.Total_Medals_Earned = Total_Medals_Earned_;
        //        GS.Level_Data = Level_Data_;
        //        d.Log("LoadData().Load() Binary Save Load Finished.", true);
        //        return true;
        //    }
        //    else
        //    {
        //        d.Log("LoadData().Load() Binary Save Load Finished.", true);
        //        return false;
        //    }
        //}
        //else
        //{
        //    d.Log("LoadData().Load() Binary Save Load Finished.", true);
        //    return false;
        //}
        #endregion
    }
    internal virtual void Update()
    {
        // Update the database with current data.
        // ( Overidden by derived class )
    }
    internal void Open_Binary_File()
    {
        try
        {
            d.Log("Attempting to open Binary File: " + Path + File_Name, false);
            if (!File.Exists(Path + File_Name))
            {
                file = File.Create(Path + File_Name);
                d.Log("Binary File doesn't exist, creating: " + Path + File_Name, false);
            }
            if (file == null)
            {
                file = File.Open(Path + File_Name, FileMode.Open);
                d.Log("Binary File opened successfully: " + Path + File_Name, false);
            }
            else
            {
                d.Log("Binary File already open: " + Path + File_Name, false);
            }
            BinaryReader br = new BinaryReader(file);
            Modified = Convert.ToDateTime(br.ReadString());
            br = null;
        }
        catch (Exception e)
        {
            d.Log("Binary File open failed: " + Path + File_Name + " > " + e.Message, true);
        }
    }
    internal void Close_Binary_File()
    {
        try
        {
            d.Log("Attempting to close Binary File: " + Path + File_Name, false);
            if (file != null)
            {
                file.Close();
                d.Log("Binary file close successful: " + Path + File_Name, false);
            }
        }
        catch (Exception e)
        {
            d.Log("Binary File close failed: " + Path + File_Name + " > " + e.Message, true);
        }
    }
}

public class Store : Local_Cache
{
    public Store()
    {
        File_Name = "Store_Save.dat";
        Open_Binary_File();
    }
    ~Store()
    {
        Close_Binary_File();
    }

    public Dictionary<string, float> d_Store = new Dictionary<string, float>();

    MySqlDataReader reader = null;

    /// <summary>
    /// Save to binary file
    /// </summary>
    public override void Save()
    {
        d.Log("Attempting to save Store contents to binary file: " + Path + File_Name, false);
        base.Save();
        // Save Store to binary file
        try
        {
            BinaryWriter bw = new BinaryWriter(file);
            bw.Write(Modified.ToString());
            bw.Write(d_Store.Count);
            foreach (var pair in d_Store)
            {
                bw.Write(pair.Key);
                bw.Write(pair.Value);
            }
            d.Log("Save Store contents to binary file successful: " + Path + File_Name, false);
        }
        catch (SerializationException sEx)
        {
            d.Log("Save Store contents to binary file failed: " + Path + File_Name + " > " + sEx.Message, true);
        }
    }
    /// <summary>
    /// Load from database unless the local cache has the data 
    /// </summary>
    public override void Load()
    {
        int Record_Count = 0;
        base.Load();
        // Load Store from database if more up to date than local cache
        DateTime DB_Date = Convert.ToDateTime(getDatabaseValue<string>("Store_Updated","SELECT Store_Updated FROM tbl_Last_Updated;"));
        d.Log("Loading Store : Local Cache > " + Modified + ", Database > " + DB_Date + ((Modified >= DB_Date) ? 
            " | Attempting to restore Store from local cache." : " | Attempting to restore Store from database."), false);
        if (Modified >= DB_Date)
        {
            #region Use local cache data
            try
            {
                BinaryReader br = new BinaryReader(file);
                Modified = Convert.ToDateTime(br.ReadString());
                Record_Count = br.ReadInt32();
                if (Record_Count > 0)
                    d_Store.Clear();
                for (int i = 0; i < Record_Count; i++)
                {
                    d_Store.Add(br.ReadString(), br.ReadSingle());
                }
                br = null;
            }
            catch (Exception e)
            {
                d.Log("Loading from local cache failed: " + e.Message + " Attempting to load from database instead.", true);
                try
                {
                    reader = queryDatabase("SELECT * FROM tbl_Store WHERE In_Use = 1;");
                    if (reader == null)
                    {
                        d.Log("Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " +
                            "| UNABLE TO LOAD STORE!", true);
                        return;
                    }
                    while (reader.Read())
                    {
                        d_Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
                    }
                    d.Log("Loading from database successful.", false);
                }
                catch (Exception ex)
                {
                    d.Log("Loading from database failed: " + ex.Message + " | UNABLE TO LOAD STORE!", true);
                }
            }
            d.Log("Loading from local cache successful: " + Path + File_Name, false);
            #endregion
        }
        else
        {
            #region Use Database data
            try
            {
                reader = queryDatabase("SELECT * FROM tbl_Store WHERE In_Use = 1;");
                if (reader == null)
                {
                    d.Log("Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " + 
                        "Attempting to load from local cache instead.", true);
                    try
                    {
                        // Use local cache data instead
                        BinaryReader br = new BinaryReader(file);
                        Modified = Convert.ToDateTime(br.ReadString());
                        Record_Count = br.ReadInt32();
                        if (Record_Count > 0)
                            d_Store.Clear();
                        for (int i = 0; i < Record_Count; i++)
                        {
                            d_Store.Add(br.ReadString(), br.ReadSingle());
                        }
                        br = null;
                    }
                    catch (Exception e)
                    {
                        d.Log("Loading from local cache failed: " + e.Message + " | UNABLE TO LOAD STORE!", true);
                    }
                    return;
                }
                while (reader.Read())
                {
                    d_Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
                }
                // Set the Modified date to reflect the new data set and save it to binary
                Modified = DB_Date;
                Save();
                d.Log("Loading from database successful.", false);
            }
            catch (Exception e)
            {
                d.Log("Loading from database failed: " + e.Message, true);
            }
            #endregion
        }
    }
    //Update function not necessary for this class.
}
public class Leaderboard : Local_Cache
{
    public Leaderboard()
    {
        File_Name = "Leaderboard_Save.dat";
        Open_Binary_File();
    }
    ~Leaderboard()   
    {
        Close_Binary_File();
    }

    internal Dictionary<string, float> d_Leaderboard = new Dictionary<string, float>();

    MySqlDataReader reader = null;

    public override void Save()
    {
        base.Save();
        reader = queryDatabase("SELECT * FROM tbl_Highscores;");
        while (reader.Read())
        {
            d.Log("Saving leaderboard data..", false);
            d_Leaderboard.Clear();
            d_Leaderboard.Add(reader["Player_Name"].ToString(), (float)reader["Score"]);
            break;
        }
    }
    public override void Load()
    {

    }

    // These need work:
    /// <summary>
    /// Submit Score and get the updated Leaderboard.
    /// </summary>
    /// <param name="Score">Score the Player achieved. (float)</param>
    /// <param name="Player_Name">Player's name. (string)</param>
    public Dictionary<string, float> leaderboard(float Score, string Player_Name)
    {
        MySqlDataReader reader = queryDatabase("SELECT * FROM tbl_Highscores WHERE Player_Name = '" + Player_Name + "';");
        if (reader == null)
            return (d_Leaderboard);
        if (reader.Read())
        {
            reader = queryDatabase("UPDATE tbl_Highscores SET Score = " + Score + " WHERE Player_Name = '" + Player_Name + "';");
        }
        else
        {
            reader = queryDatabase("INSERT INTO tbl_Highscores (Player_Name, Score) SELECT '" + Player_Name + "', " + Score + ";");
        }
        //LoadData(false, true, false);
        return (d_Leaderboard);
    }
    /// <summary>
    /// Updates and returns the Leaderboard from the local cache, this can be updated using LoadData and selecting true for the Leaderboard bool.
    /// </summary>
    /// <returns>The local Leaderboard cache.</returns>
    public Dictionary<string, float> leaderboard()
    {
        //LoadData(false, true, false);
        return (d_Leaderboard);
    }
}
public class Game_State : Local_Cache
{
    public Game_State()
    {
        File_Name = "Game_State_Save.dat";
        Open_Binary_File();
    }
    ~Game_State()
    {
        Close_Binary_File();
    }

    public override void Save()
    {
        FileStream file;
        if (!File.Exists(Path + File_Name))
            file = File.Create(Path + File_Name);
        else
            file = File.Open(Path + File_Name, FileMode.Open);

        try
        {
            d.Log("Creating save file with the following data:", false);
            BinaryFormatter bf = new BinaryFormatter();
            BinaryWriter bw = new BinaryWriter(file);
            bw.Write(Total_Medals_Earned);
            bw.Write(Current_Medals);
            bw.Write(Current_Gems);
            bw.Write(Total_Gems_Earned);
            bw.Write(DateTime.Now.ToString());
            d.Log("Total_Medals_Earned: " + Total_Gems_Earned + ", Current_Medals: " + Current_Medals + ", Current_Gems: " + Current_Gems + ", Total_Gems_Earned: " + 
                Total_Gems_Earned + ", Modified: " + DateTime.Now.ToString(), true);
            foreach (Level_Info level in Level_Data)
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

    // The amount of Medals the Player currently has.
    private byte current_Medals = 0;
    public byte Current_Medals { get { return this.current_Medals; } set { this.current_Medals = value; Modified = DateTime.Now; } }

    // The total amount of Medals the Player has earned.
    //This is probably not necessary but I wanted to put it in just in case we wanted to give the Player the ability to trade them.
    private byte total_Medals_Earned = 0;
    public byte Total_Medals_Earned { get { return this.total_Medals_Earned; } set { this.total_Medals_Earned = value; Modified = DateTime.Now; } }

    // The amount of Gems currently in the Players inventory.
    private int current_Gems = 0;
    public int Current_Gems { get { return this.current_Gems; } set { this.current_Gems = value; Modified = DateTime.Now; } }

    // The lifetime total amount of Gems the Player has earned.
    private int total_Gems_Earned = 0;
    public int Total_Gems_Earned { get { return this.total_Gems_Earned; } set { this.total_Gems_Earned = value; Modified = DateTime.Now; } }

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