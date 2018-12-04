using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{

    public bool mute = false;
    public static float volume;

    private AudioSource AS;

    public GameObject MusicToggleButton;
    void Start()
    {
        //// Makes sure the game object is not deleted when the user moves to a new scene
        //DontDestroyOnLoad(transform.gameObject);
        AS = this.GetComponent<AudioSource>();
        AS.volume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        PlayerPrefs.GetInt("MusicToggle", 1);
    }

    public void muteMusic()
    {
        if(PlayerPrefs.GetFloat("MusicVolume",1.0f)==1.0f)
        {
            PlayerPrefs.SetFloat("MusicVolume", 0.0f);
            AS.volume = 0.0f;
            PlayerPrefs.SetInt("MusicToggle", 0);
            MusicToggleButton.GetComponent<MenuController>().toggleMusic();
        }
        else if (PlayerPrefs.GetFloat("MusicVolume", 1.0f) == 0.0f)
        {
            PlayerPrefs.SetFloat("MusicVolume", 1.0f);
            AS.volume = 1.0f;
            PlayerPrefs.SetInt("MusicToggle", 1);
            MusicToggleButton.GetComponent<MenuController>().toggleMusic();
        }
        //if (mute == false)
        //{
        //    AudioListener.volume = 1.0f;
        //}
        //else if (mute == true)
        //{
        //    AudioListener.volume = 0.0f;
        //}
    }
}
