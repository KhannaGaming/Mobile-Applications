using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource AS;

    private void Awake()
    {
        AS = GetComponent<AudioSource>();
    }

    public void Toggle()
    {
        AS.enabled = !AS.enabled;
    }
}
