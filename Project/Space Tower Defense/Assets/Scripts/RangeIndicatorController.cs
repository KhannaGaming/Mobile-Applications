﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicatorController : MonoBehaviour {

    private float range = 0;



	
	// Update is called once per frame
	void Update () {
        if (transform.parent.childCount > 2)
        {
            if (range != transform.parent.GetChild(2).GetComponent<Turret>().range)
            {
                adjustRangeIndicator();
            }
        }
	}

    void adjustRangeIndicator()
    {
        transform.localScale = new Vector3(transform.parent.GetChild(2).GetComponent<Turret>().range*4, transform.parent.GetChild(2).GetComponent<Turret>().range*4,0f);
        range = transform.parent.GetChild(2).GetComponent<Turret>().range;
    }
}
