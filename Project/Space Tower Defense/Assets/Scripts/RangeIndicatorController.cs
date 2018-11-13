using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicatorController : MonoBehaviour {




	// Use this for initialization
	void Start () {
        transform.localScale = new Vector3(transform.parent.GetComponent<Turret>().range * 10f, transform.parent.GetComponent<Turret>().range * 10f,0f);
		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
