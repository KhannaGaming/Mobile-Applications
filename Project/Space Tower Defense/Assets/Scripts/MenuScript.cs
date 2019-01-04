using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    public List<GameObject> MenuItems;
   public  GameObject MusicToggle;
    bool musicToggle;
    public List<GameObject> TowerSlots;
    private void Start()
    {
        //MusicToggle = GameObject.Find("Music On/Off");

        if (PlayerPrefs.GetInt("MusicToggle", 1) == 1)
        {
            musicToggle = true;
        }
        else
        {
            musicToggle = false;
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
        PlayerPrefs.SetString("Level", level);
        SceneManager.LoadScene("Loading");
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
}
