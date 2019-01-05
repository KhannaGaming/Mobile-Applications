using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollWith : MonoBehaviour {

    private GameObject text;

	// Use this for initialization
	void Start () {
        text = GameObject.Find("LeaderboardText");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(transform.position.x, text.transform.position.y, transform.position.z);
	}
}
