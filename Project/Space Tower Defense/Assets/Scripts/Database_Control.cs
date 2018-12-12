﻿using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;
using System.Runtime.CompilerServices;
using System.Globalization;

public class Database_Control : MonoBehaviour {

    public Game_State GameState = new Game_State();
    public Leaderboard leaderboard = new Leaderboard();
    public Store store = new Store();

    private Debug_Log d;

    #region General Initialisation
    void Awake()
    {
        d = new Debug_Log(Application.persistentDataPath + "/");
        //Initialising the local cache in awake due to constructor
        d.DUI = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Unique Client ID: " + d.DUI);
        store.d = d;
        leaderboard.d = d;
        GameState.d = d;
    }
    #endregion
}

public class Database_Interaction
{
    #region Database Variables
    private MySqlConnection connection = null;
    private MySqlCommand command = null;
    private string connectionString = "Server = den1.mysql5.gear.host; port = 3306; Database = stddb; User = stdclient; Password = '8ch8J5PPRRCFKp6!'; SslMode=none;";
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
            {
                return (T)reader[value];
            }
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
    private bool Override = false;//Change this to true if you want to print all logs to the debug_log text file
    private bool Reset_On_Start = true;
    internal string Path = "";
    internal string DUI = "";

    public Debug_Log(string Path_ = "")
    {
        Path = Path_;
        if (Reset_On_Start)
            File.WriteAllText(Path + "debug_log.txt", String.Empty);
    }

    public void Log(bool success, string output, bool toConsole, [CallerLineNumber] int LineNumber = 0, [CallerMemberName] string Caller = null)
    {
        if (!Allow_Logs) return;
        if (toConsole || Override) Debug.Log(output);
        using (StreamWriter writer = new StreamWriter(Path + "debug_log.txt", true))
        {
            writer.WriteLine(((success) ? " " : "!") + DateTime.Now + " > Caller: " + Caller + ", Line Number: " + LineNumber + ": " + output);
        }
    }
    public List<string> Retrieve()
    {
        if (!Allow_Logs) return null;
        List<string> temp = new List<string>();
        using (StreamReader reader = new StreamReader(Path + "debug_log.txt"))
        {
            temp.Add(reader.ReadLine());
        }
        return temp;
    }
}
public class Local_Cache : Database_Interaction
{
    internal Debug_Log d = new Debug_Log();
    
    internal string File_Name = "";

    internal FileStream file;

    // This is the time of the last game state update currently held in the games memory.
    internal DateTime Modified = new DateTime();

    public virtual void Save()
    {
        // Save data to binary file.
        // ( Overidden by derived class )
    }
    /// <summary>
    /// Load data from local cache unless it's older than the database.
    /// </summary>
    public virtual void Load()
    {
        // Load from either the database or binary file (whichever is the most up to date).
        // ( Overidden by derived class )
    }
    public virtual void Update(string Key, float Value)
    {
        // Update the database with current data.
        // ( Overidden by derived class )
    }
    internal void Get_Modified_Date()
    {
        try
        {
            d.Log(true, "Attempting to get Modified date from local cache file.", false);
            using (BinaryReader br = new BinaryReader(File.OpenRead(d.Path + File_Name)))
            {
                Modified = Convert.ToDateTime(br.ReadString());
            }
            d.Log(true, "Successfully read Modified date from local cache file.", false);
        }
        catch (Exception e)
        {
            d.Log(false, "Couldn't get Modified date from local cache file: " + d.Path + File_Name + " > " + e.Message, true);
        }
    }
}

public class Store : Local_Cache
{
    public Store()
    {
        File_Name = "Store_Save.dat";
    }

    public Dictionary<string, float> d_Store = new Dictionary<string, float>();

    MySqlDataReader reader = null;

    /// <summary>
    /// Save to binary file
    /// </summary>
    public override void Save()
    {
        d.Log(true, "Attempting to save Store contents to binary file: " + d.Path + File_Name, false);
        // Save Store to binary file
        try
        {
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(d.Path + File_Name)))
            {
                bw.Write(Modified.ToString());
                bw.Write(d_Store.Count);
                foreach (var pair in d_Store)
                {
                    bw.Write(pair.Key);
                    bw.Write(pair.Value);
                }
            }
            d.Log(true, "Save Store contents to binary file successful: " + d.Path + File_Name, false);
        }
        catch (SerializationException sEx)
        {
            d.Log(false, "Save Store contents to binary file failed: " + d.Path + File_Name + " > " + sEx.Message, true);
        }
    }
    /// <summary>
    /// Load from database unless the local cache has the data 
    /// </summary>
    public override void Load()
    {
        Get_Modified_Date();
        int Record_Count = 0;
        DateTime DB_Date = getDatabaseValue<DateTime>("MAX(DT_Stamp)", "SELECT MAX(DT_Stamp) FROM tbl_Store;");
        d.Log(true, "Loading Store : Local Cache > " + Modified + ", Database > " + DB_Date + ((Modified >= DB_Date) ? 
            " | Attempting to restore Store from local cache." : " | Attempting to restore Store from database."), false);
        if (Modified >= DB_Date)
        {
            #region Use local cache data
            try
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(d.Path + File_Name)))
                {
                    Modified = Convert.ToDateTime(br.ReadString());
                    Record_Count = br.ReadInt32();
                    if (Record_Count > 0)
                        d_Store.Clear();
                    for (int i = 0; i < Record_Count; i++)
                    {
                        d_Store.Add(br.ReadString(), (float)br.ReadSingle());
                    }
                }
            }
            catch (Exception e)
            {
                d.Log(false, "Loading from local cache failed: " + e.Message + " Attempting to load from database instead.", true);
                try
                {
                    reader = queryDatabase("SELECT * FROM tbl_Store WHERE In_Use = 1;");
                    if (reader == null)
                    {
                        d.Log(false,"Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " +
                            "| UNABLE TO LOAD STORE!", true);
                        return;
                    }
                    d_Store.Clear();
                    while (reader.Read())
                    {
                        d_Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
                    }
                    d.Log(true,"Loading from database successful.", false);
                    return;
                }
                catch (Exception ex)
                {
                    d.Log(false,"Loading from database failed: " + ex.Message + " | UNABLE TO LOAD STORE!", true);
                }
            }
            d.Log(true,"Loading from local cache successful: " + d.Path + File_Name, false);
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
                    d.Log(false,"Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " + 
                        "Attempting to load from local cache instead.", true);
                    try
                    {
                        // Use local cache data instead
                        using (BinaryReader br = new BinaryReader(File.OpenRead(d.Path + File_Name)))
                        {
                            Modified = Convert.ToDateTime(br.ReadString());
                            Record_Count = br.ReadInt32();
                            if (Record_Count > 0)
                                d_Store.Clear();
                            for (int i = 0; i < Record_Count; i++)
                            {
                                d_Store.Add(br.ReadString(), (float)br.ReadSingle());
                            }
                        }
                        d.Log(true, "Loading from local cache successful.", false);
                    }
                    catch (Exception e)
                    {
                        d.Log(false,"Loading from local cache failed: " + e.Message + " | UNABLE TO LOAD STORE!", true);
                    }
                    return;
                }
                d_Store.Clear();
                while (reader.Read())
                {
                    d_Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
                }
                // Set the Modified date to reflect the new data set and save it to binary
                Modified = DB_Date;
                Save();
                d.Log(true,"Loading from database successful.", false);
            }
            catch (Exception e)
            {
                d.Log(false,"Loading from database failed: " + e.Message, true);
            }
            #endregion
        }
        // For testing purposes:
        d.Log(true, "Store Contents: ", false);
        foreach (KeyValuePair<string,float> entry in d_Store)
        {
            d.Log(true, "Name: " + entry.Key + " Value: " + entry.Value, false);
        }
    }
    // Update function not necessary for this class.
    
    // Access functions
    public Dictionary<string,float> store()
    {
        return (d_Store);
    }
}
public class Leaderboard : Local_Cache
{
    public Leaderboard()
    {
        File_Name = "Leaderboard_Save.dat";
    }

    internal Dictionary<string, float> d_Leaderboard = new Dictionary<string, float>();

    MySqlDataReader reader = null;

    /// <summary>
    /// Save to binary file
    /// </summary>
    public override void Save()
    {
        d.Log(true, "Attempting to save Leaderboad contents to binary file: " + d.Path + File_Name, false);
        // Save Leaderboard to binary file
        try
        {
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(d.Path + File_Name)))
            {
                bw.Write(Modified.ToString());
                bw.Write(d_Leaderboard.Count);
                foreach (var pair in d_Leaderboard)
                {
                    bw.Write(pair.Key);
                    bw.Write(pair.Value);
                }
            }
            d.Log(true, "Save Leaderboard contents to binary file successful: " + d.Path + File_Name, false);
        }
        catch (Exception e)
        {
            d.Log(false, "Save Leaderboard contents to binary file failed: " + d.Path + File_Name + " > " + e.Message, true);
        }
    }
    /// <summary>
    /// Load from database unless the local cache has the data
    /// </summary>
    public override void Load()
    {
        Get_Modified_Date();
        int Record_Count = 0;
        DateTime DB_Date = getDatabaseValue<DateTime>("MAX(DT_Stamp)", "SELECT MAX(DT_Stamp) FROM tbl_Highscores;");
        d.Log(true, "Loading Leaderboard : Local Cache > " + Modified + ", Database > " + DB_Date + ((Modified >= DB_Date) ?
            " | Attempting to restore Leaderboard from local cache." : " | Attempting to restore Leaderboard from database."), false);
        if (Modified >= DB_Date)
        {
            #region Use Local cache data
            try
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(d.Path + File_Name)))
                {
                    Modified = Convert.ToDateTime(br.ReadString());
                    Record_Count = br.ReadInt32();
                    if (Record_Count > 0)
                        d_Leaderboard.Clear();
                    for (int i = 0; i < Record_Count; i++)
                    {
                        d_Leaderboard.Add(br.ReadString(), (float)br.ReadSingle());
                    }
                }
            }
            catch (Exception e)
            {
                d.Log(false, "Loading from local cache failed: " + e.Message + " Attempting to load from database instead.", true);
                try
                {
                    reader = queryDatabase("SELECT * FROM tbl_Highscores ORDER BY Score ASC;");
                    if (reader == null)
                    {
                        d.Log(false, "Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " +
                            "| UNABLE TO LOAD LEADERBOARD!", true);
                        return;
                    }
                    d_Leaderboard.Clear();
                    while (reader.Read())
                    {
                        d_Leaderboard.Add(reader["Player_Name"].ToString(), (float)reader["Score"]);
                    }
                    Modified = DB_Date;
                    d.Log(true, "Loading from database successful.", false);
                    return;
                }
                catch (Exception ex)
                {
                    d.Log(false, "Loading from database failed: " + ex.Message + " | UNABLE TO LOAD LEADERBOARD!", true);
                }
            }
            d.Log(true, "Loading from local cache successful: " + d.Path + File_Name, false);
            #endregion
        }
        else
        {
            #region Use Database data
            try
            {
                reader = queryDatabase("SELECT * FROM tbl_Highscores ORDER BY Score ASC;");
                if (reader == null)
                {
                    d.Log(false, "Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " +
                        "Attempting to load from local cache instead.", true);
                    try
                    {
                        // Use local cache data instead
                        using (BinaryReader br = new BinaryReader(File.OpenRead(d.Path + File_Name)))
                        {
                            Modified = Convert.ToDateTime(br.ReadString());
                            Record_Count = br.ReadInt32();
                            if (Record_Count > 0)
                                d_Leaderboard.Clear();
                            for (int i = 0; i < Record_Count; i++)
                            {
                                d_Leaderboard.Add(br.ReadString(), (float)br.ReadSingle());
                            }
                        }
                        d.Log(true, "Loading from local cache successful.", false);
                    }
                    catch (Exception e)
                    {
                        d.Log(false, "Loading from local cache failed: " + e.Message + " | UNABLE TO LOAD LEADERBOARD!", true);
                    }
                    return;
                }
                d_Leaderboard.Clear();
                while (reader.Read())
                {
                    d_Leaderboard.Add(reader["Player_Name"].ToString(), (float)reader["Score"]);
                }
                // Set the Modified date to reflect the new data set and save it to binary
                Modified = DB_Date;
                Save();
                d.Log(true, "Loading from database successful.", false);
            }
            catch (Exception e)
            {
                d.Log(false, "Loading from database failed: " + e.Message, true);
            }
            #endregion
        }
        // For testing purposes:
        d.Log(true, "Leaderboard Contents: ", false);
        foreach (KeyValuePair<string, float> entry in d_Leaderboard)
        {
            d.Log(true, "Name: " + entry.Key + " Score: " + entry.Value, false);
        }
    }
    /// <summary>
    /// Update database records
    /// </summary>
    /// <param name="Key">The name of the player.</param>
    /// <param name="Value">The score achieved.</param>
    public override void Update(string Key, float Value)
    {
        try
        {
            d.Log(true, "Attempting to update the database with the following data > Player_Name: '" + Key + "', Score: " + Value, false);
            // Check database to see if we already hold records with this players information
            reader = queryDatabase("SELECT * FROM tbl_Highscores WHERE Unique_Identifier = '" + d.DUI + "';");
            if (reader == null)
            {
                d.Log(false, "The MySqlDataReader returned null, please ensure you have data in the database and you are connected to the internet! | UNABLE TO UPDATE DATABASE!", true);
                return;
            }
            if (reader.HasRows)
            {
                // Data was found, update it
                d.Log(true, "Leaderboard data for user: " + Key + " was found, updating record with a new Score of " + Value, false);
                reader = queryDatabase("UPDATE tbl_Highscores " + 
                                        "SET Player_Name = '" + Key + "', " +
                                            "Score = " + Value + ", " +
                                            "DT_Stamp = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " + 
                                            "WHERE Unique_Identifier = '" + d.DUI + "';");
                d.Log(true, "Leaderboard data successfull updated > Player Name: " + Key + ", Score: " + Value + ", Date Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", Unique Identifier: " + d.DUI, false);
            }
            else
            {
                // No data was found, insert into
                d.Log(true, "Leaderboard data for user: " + Key + " was not found, inserting a new record with a new Score of " + Value, false);
                reader = queryDatabase("INSERT INTO tbl_Highscores " +
                                            "(Player_Name, Score, DT_Stamp, Unique_Identifier) " +
                                        "SELECT '" + Key + "', " +
                                            Value + ", " +
                                            "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " + 
                                            "'" + d.DUI + "';");
                d.Log(true, "Leaderboard data successfull inserted > Player Name: " + Key + ", Score: " + Value + ", Date Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", Unique Identifier: " + d.DUI, false);
            }
        }
        catch (Exception e)
        {
            d.Log(false,"Update Leaderboard data failed: " + e.Message, true);
        }
    }

    // Access functions
    /// <summary>
    /// Submit Score and get the updated Leaderboard.
    /// </summary>
    /// <param name="Score">Score the Player achieved. (float)</param>
    /// <param name="Player_Name">Player's name. (string)</param>
    public Dictionary<string, float> leaderboard(float Score, string Player_Name)
    {
        Update(Player_Name, Score);
        Load();

        return (d_Leaderboard);
    }
    /// <summary>
    /// Updates and returns the Leaderboard from the local cache, this can be updated using LoadData and selecting true for the Leaderboard bool.
    /// </summary>
    /// <returns>The local Leaderboard cache.</returns>
    public Dictionary<string, float> leaderboard()
    {
        Load();

        return (d_Leaderboard);
    }
}
public class Game_State : Local_Cache
{
    public Game_State()
    {
        File_Name = "Game_State_Save.dat";
        if (!File.Exists(d.Path + File_Name))
            File.Create(d.Path + File_Name);
    }

    MySqlDataReader reader = null;

    internal int Save_ID = 0;

    /// <summary>
    /// Save to binary file
    /// </summary>
    public override void Save()
    {
        d.Log(true, "Attempting to save Game State contents to binary file: " + d.Path + File_Name, false);
        // Save Game State to binary file
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(d.Path + File_Name)))
            {
                bw.Write(Modified.ToString());
                bw.Write(Current_Medals);
                bw.Write(Total_Medals_Earned);
                bw.Write(Current_Gems);
                bw.Write(Total_Gems_Earned);
                bw.Write(Level_Data.Count);
                d.Log(true, "Successfully saved Game State of > [Total_Medals_Earned: " + Total_Medals_Earned + " | Current_Medals: " + Current_Medals +
                    " | Total_Gems_Earned: " + Total_Gems_Earned + " | Current_Gems: " + Current_Gems + "] to binary file: " + d.Path + File_Name, false);
                foreach (Level_Info level in Level_Data)
                {
                    bf.Serialize(file, level);
                    d.Log(true, "Level #" + level.Number + " - " + level.Name + " (" + level.Medals + " Medals Earned) saved to binary file: " + d.Path + File_Name, true);
                }
            }
            d.Log(true, "Save Game State contents to binary file successful: " + d.Path + File_Name, false);
        }
        catch (SerializationException e)
        {
            d.Log(false, "Save Game State contents to binary file failed: " + d.Path + File_Name + " > " + e.Message, true);
        }
    }
    /// <summary>
    /// Load from database unless the local cache has the data
    /// </summary>
    public override void Load()
    {
        Get_Modified_Date();
        DateTime DB_Date = getDatabaseValue<DateTime>("MAX(DT_Stamp)", "SELECT MAX(DT_Stamp) FROM tbl_Save_Data WHERE Unique_Identifier = '" + d.DUI + "';");
        d.Log(true, "Loading Game State : Local Cache > " + Modified + ", Database > " + DB_Date + ((Modified >= DB_Date) ?
            " | Attempting to restore Game State from local cache." : " | Attempting to restore Game State from database."), false);
        if (Modified >= DB_Date)
        {
            #region Use Local cache data
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (BinaryReader br = new BinaryReader(File.OpenRead(d.Path + File_Name)))
                {
                    Modified = Convert.ToDateTime(br.ReadString());
                    Current_Medals = br.ReadByte();
                    Total_Medals_Earned = br.ReadByte();
                    Current_Gems = br.ReadInt32();
                    Total_Gems_Earned = br.ReadInt32();
                    Level_Data.Clear();
                    int Level_Count = br.ReadInt32();
                    for (int i = 0; i < Level_Count; i++)
                    {
                        Level_Data.Add((Level_Info)bf.Deserialize(file));
                    }
                }
            }
            catch (Exception e)
            {
                d.Log(false, "Loading from local cache failed: " + e.Message + " Attempting to load from database instead.", true);
                try
                {
                    reader = queryDatabase("SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + d.DUI + "';");
                    if (reader == null)
                    {
                        d.Log(false, "Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " +
                            "| UNABLE TO LOAD GAME STATE!", true);
                        return;
                    }
                    if (reader.Read())
                    {
                        Current_Medals = reader.GetByte("Current_Medals");
                        Total_Medals_Earned = reader.GetByte("Total_Medals_Earned");
                        Current_Gems = reader.GetInt32("Current_Gems");
                        Total_Gems_Earned = reader.GetInt32("Total_Gems_Earned");
                        int Save_ID = reader.GetInt32("Save_ID");
                        Level_Data.Clear();
                        reader = queryDatabase("SELECT " +
                                                "tbl_Level_Save_Data.Medals_Earned AS ME, " +
                                                "tbl_Map_Data.Level_Name AS LN, " +
                                                "tbl_Map_Data.Level_Number AS MI " +
                                            "FROM tbl_Level_Save_Data " +
                                            "LEFT JOIN tbl_Map_Data " +
                                                "ON tbl_Level_Save_Data.Map_ID = tbl_Map_Data.Level_Number " +
                                            "WHERE tbl_Level_Save_Data.Save_ID = " + Save_ID + ";");
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                                Level_Data.Add(new Level_Info(reader["LN"].ToString(),reader.GetByte("MI"),reader.GetByte("ME")));
                        }
                    }
                    else
                    {
                        d.Log(false, "Loading from database failed: The MySqlDataReader returned no records | UNABLE TO LOAD GAME STATE!", true);
                        return;
                    }
                    Modified = DB_Date;
                    d.Log(true, "Loading from database successful.", false);
                    return;
                }
                catch (Exception ex)
                {
                    d.Log(false, "Loading from database failed: " + ex.Message + " | UNABLE TO LOAD GAME STATE!", true);
                }
            }
            d.Log(true, "Loading from local cache successful: " + d.Path + File_Name, false);
            #endregion
        }
        else
        {
            #region Use Database data
            try
            {
                reader = queryDatabase("SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + d.DUI + "';");
                if (reader == null)
                {
                    d.Log(false, "Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " +
                        "Attempting to load from local cache instead.", true);
                    try
                    {
                        // Use local cache data instead
                        BinaryFormatter bf = new BinaryFormatter();
                        using (BinaryReader br = new BinaryReader(File.OpenRead(d.Path + File_Name)))
                        {
                            Modified = Convert.ToDateTime(br.ReadString());
                            Current_Medals = br.ReadByte();
                            Total_Medals_Earned = br.ReadByte();
                            Current_Gems = br.ReadInt32();
                            Total_Gems_Earned = br.ReadInt32();
                            Level_Data.Clear();
                            int Level_Count = br.ReadInt32();
                            for (int i = 0; i < Level_Count; i++)
                            {
                                Level_Data.Add((Level_Info)bf.Deserialize(file));
                            }
                            d.Log(true, "Loading from local cache successful.", false);
                        }
                    }
                    catch (Exception e)
                    {
                        d.Log(false, "Loading from local cache failed: " + e.Message + " | UNABLE TO LOAD GAME STATE!", true);
                    }
                    return;
                }
                if (reader.Read())
                {
                    Current_Medals = reader.GetByte("Current_Medals");
                    Total_Medals_Earned = reader.GetByte("Total_Medals_Earned");
                    Current_Gems = reader.GetInt32("Current_Gems");
                    Total_Gems_Earned = reader.GetInt32("Total_Gems_Earned");
                    int Save_ID = reader.GetInt32("Save_ID");
                    Level_Data.Clear();
                    reader = queryDatabase("SELECT " +
                                            "tbl_Level_Save_Data.Medals_Earned AS ME, " +
                                            "tbl_Map_Data.Level_Name AS LN, " +
                                            "tbl_Map_Data.Level_Number AS MI " +
                                        "FROM tbl_Level_Save_Data " +
                                        "LEFT JOIN tbl_Map_Data " +
                                            "ON tbl_Level_Save_Data.Map_ID = tbl_Map_Data.Level_Number " +
                                        "WHERE tbl_Level_Save_Data.Save_ID = " + Save_ID + ";");
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                            Level_Data.Add(new Level_Info(reader["LN"].ToString(), reader.GetByte("MI"), reader.GetByte("ME")));
                    }
                }
                else
                {
                    d.Log(false, "Loading from database failed: The MySqlDataReader returned no records | UNABLE TO LOAD GAME STATE!", true);
                    return;
                }
                Modified = DB_Date;
                d.Log(true, "Loading from database successful.", false);
                return;
            }
            catch (Exception ex)
            {
                d.Log(false, "Loading from database failed: " + ex.Message + " | UNABLE TO LOAD GAME STATE!", true);
            }
            #endregion
        }
        // For testing purposes:
        d.Log(true, "Game State Contents: ", false);
        d.Log(true, "Current Medals: " + Current_Medals + ", Total Medals Earned: " + total_Medals_Earned +
            ", Current Gems: " + Current_Gems + ", Total Gems Earned: " + Total_Gems_Earned + 
            ", Modified DateTime: " + Modified, false);
        foreach (Level_Info level in Level_Data)
        {
            d.Log(true, "Level #" + level.Number + ", '" + level.Name + "', Medals: " + level.Medals, false);
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