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
		if(transform.parent.GetChild(1).name  == "Turret 1a")
        {
            ParticleSystem.MainModule main = ps.main;
            main.startColor =  Color.red;
                    }
	}
}
