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
            if (transform.parent.GetChild(1).tag == "StandardTurret")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.red;
            }
            else if (transform.parent.GetChild(1).name == "ElectricityTurret")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.blue;
            }
            else if (transform.parent.GetChild(1).name == "RocketTurret")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.yellow;
            }
            else if (transform.parent.GetChild(1).name == "LaserTurret")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.magenta;
            }
            else if (transform.parent.GetChild(1).name == "SniperTurret")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.white;
            }
        }
	}
}
