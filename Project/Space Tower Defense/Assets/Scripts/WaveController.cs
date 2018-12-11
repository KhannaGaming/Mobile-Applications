using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveController : MonoBehaviour
{

    // Variables
    public static int waveValue = 0; // Used to keep track of the amount of waves the player has completed
    Text waves; // Variable needed to make a connection to the wave counter text object
    public int wavesInLevel = 0;
    // Use this for initialization
    void Start()
    {
        // Makes a reference to the wave counter text object
        waves = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Sets the text value to "Waves Completed: waveValue variable"
        waves.text = "Current wave: " + waveValue + "/" + wavesInLevel;
    }

    public void increaseWave()
    {
        waveValue++;
    }
    public void maxWave(int maxWave)
    {
        wavesInLevel = maxWave;
    }
}