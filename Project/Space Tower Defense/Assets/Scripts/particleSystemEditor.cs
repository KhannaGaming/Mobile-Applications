using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleSystemEditor : MonoBehaviour {

    private ParticleSystem ps;

	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.parent.childCount > 1)
        {
            if (transform.parent.GetChild(1).name == "Turret 1a")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.red;
            }
            else if (transform.parent.GetChild(1).name == "Turret 2a")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.blue;
            }
            else if (transform.parent.GetChild(1).name == "Turret 3a")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.yellow;
            }
            else if (transform.parent.GetChild(1).name == "Turret 4a")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.magenta;
            }
        }
	}
}
