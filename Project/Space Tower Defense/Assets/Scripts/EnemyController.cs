using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    public NavMeshAgent NMA;
    public GameObject EndPath;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("num children:"+EndPath.transform.childCount);
        int childNumber = Random.Range(0, 3);
        Debug.Log(childNumber);
       NMA.SetDestination(EndPath.transform.GetChild(childNumber).transform.position);
	}
}
