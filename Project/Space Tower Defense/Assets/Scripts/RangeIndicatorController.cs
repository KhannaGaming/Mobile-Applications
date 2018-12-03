using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicatorController : MonoBehaviour {

    private float range = 0;


	// Use this for initialization
	void Start () {
        adjustRangeIndicator();
	}
	
	// Update is called once per frame
	void Update () {
        if(range != transform.parent.GetComponent<Turret>().range)
        {
            adjustRangeIndicator();
        }
	}

    void adjustRangeIndicator()
    {
        transform.localScale = new Vector3(transform.parent.GetComponent<Turret>().range*4.0f, transform.parent.GetComponent<Turret>().range*4.0f,0f);
        range = transform.parent.GetComponent<Turret>().range;
    }
}
