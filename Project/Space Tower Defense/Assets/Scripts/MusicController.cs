using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    void Awake()
    {
        // Makes sure the game object is not deleted when the user moves to a new scene
        DontDestroyOnLoad(transform.gameObject);
    }
}
