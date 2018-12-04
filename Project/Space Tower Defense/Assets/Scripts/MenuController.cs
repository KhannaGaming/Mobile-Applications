using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuController : MonoBehaviour
{
    GameObject MusicToggle;
     bool musicToggle;

    // Used for loading scenes using buttons
    // Takes a string parameter
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Used for the exit game button
    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        MusicToggle = GameObject.Find("Music On/Off");

        if (PlayerPrefs.GetInt("MusicToggle", 1) == 1)
        {
            musicToggle = true;
        }
        else
        {
            musicToggle = false;
        }

        MusicToggle.GetComponent<Toggle>().isOn = musicToggle;

    }
    private void Update()
    {
        MusicToggle.GetComponent<Toggle>().isOn = musicToggle;

    }
    public void toggleMusic()
    {
        
        MusicToggle.GetComponent<Toggle>().isOn = !MusicToggle.GetComponent<Toggle>().isOn;
    }
}
