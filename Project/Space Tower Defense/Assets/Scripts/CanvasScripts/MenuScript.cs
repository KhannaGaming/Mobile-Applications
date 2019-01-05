using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    public List<GameObject> MenuItems;
    public  GameObject MusicToggle;
    public List<GameObject> TowerSlots;
    private GameObject DBGO;
    private Database_Control DB;

    private void Start()
    {
        DBGO = GameObject.Find("DatabaseCanvas");

        DB = GameObject.Find("Database Controller").GetComponent<Database_Control>();
        //DB.GameState.Load();
        for (int i = 0; i < TowerSlots.Count; i++)
        {
        Vector3 textPos = Camera.main.WorldToScreenPoint(TowerSlots[i].transform.GetChild(0).GetChild(0).GetComponent<TowerController>().TowerParent.transform.position);
        TowerSlots[i].transform.position = textPos;
        }
    }

    public void Open(string Menu_Item_Name)
    {
        foreach (GameObject item in MenuItems)
        {
            if (item.name == Menu_Item_Name)
            {
                item.SetActive(true);
            }
            else
                item.SetActive(false);
        }
    }

    public void LoadLevel(string level)
    {
        if (level.Substring(0, 5) == "Level")
        {
            int level_Number = Convert.ToInt32(level.Substring(5, 2));
            if (level_Number == 01)
            {
                PlayerPrefs.SetString("Level", level);
                SceneManager.LoadScene("Loading");
            }
            else
            {
                try
                {
                    if (DB.GameState.Level_Data[level_Number - 2] != null)
                    {
                        PlayerPrefs.SetString("Level", level);
                        SceneManager.LoadScene("Loading");
                    }
                }
                catch (System.Exception e)
                {
                    DB.d.Log(false, "There was no data in next level. > " + e.Message, true);
                }

            }
        }
        else if(level.Substring(0, 8) == "MainMenu")
        {
            PlayerPrefs.SetString("Level", level);
            SceneManager.LoadScene("Loading");
        }
    }
    public void toggleMusic()
    {

       if( MusicToggle.GetComponent<Toggle>().isOn == false)
        {
            PlayerPrefs.SetInt("MusicToggle", 0);
           // GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().volume = 0;
        }
        else if (MusicToggle.GetComponent<Toggle>().isOn == true)
        {
            PlayerPrefs.SetInt("MusicToggle", 1);
            //GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().volume = 1;
        }
    }
    private void Update()
    {
        if (MusicToggle != null)
        {
            if (PlayerPrefs.GetInt("MusicToggle", 1) == 1)
            {
                MusicToggle.GetComponent<Toggle>().isOn = true;
            }
            else if (PlayerPrefs.GetInt("MusicToggle", 1) == 0)
            {
                MusicToggle.GetComponent<Toggle>().isOn = false;
            }
        }
    }
    public void HideButtons()
    {
        foreach (GameObject Tower_Slot in TowerSlots)
        {
            foreach (GameObject btn in Tower_Slot.transform.GetChild(0).transform.GetChild(0).GetComponent<TowerController>().Buttons_)
                btn.SetActive(false);
            
            foreach (GameObject STB in GameObject.FindGameObjectsWithTag("SpawnTowerButton"))
                STB.GetComponent<TowerController>().TowerParent.transform.GetChild(1).GetComponent<RangeIndicatorController>().switchRangeIndicatorOff();
            
        }
    }

    public void ContiueTitleButton(string level)
    {
        if (PlayerPrefs.GetString("UserName") == "")
        {
            ReturnCode check = DB.leaderboard.checkDatabase("SELECT Player_Name FROM tbl_Highscores WHERE Player_Name = '" + GameObject.Find("UserNameText").GetComponent<Text>().text + "';");
            if (check == ReturnCode.True)
            {
                GameObject.Find("UserNameExists").GetComponent<Text>().enabled = true;
                return;
            }
            else
            {
                PlayerPrefs.SetString("UserName", GameObject.Find("UserNameText").GetComponent<Text>().text);
                DontDestroyOnLoad(DBGO);
                PlayerPrefs.SetString("Level", level);
                SceneManager.LoadScene("Loading");
            }
        }
        else
        {
            DontDestroyOnLoad(DBGO);
            PlayerPrefs.SetString("Level", level);
            SceneManager.LoadScene("Loading");
        }
    }
}
