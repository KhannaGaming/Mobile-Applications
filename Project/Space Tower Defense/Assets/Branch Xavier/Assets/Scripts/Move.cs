using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    public float speed = 5.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // Move the object forward along its z axis 1 unit/second.
        transform.Translate(Vector3.forward  *speed *Time.deltaTime);


    }
}

