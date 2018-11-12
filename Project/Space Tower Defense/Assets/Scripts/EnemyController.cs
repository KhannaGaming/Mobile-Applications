using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    public  NavMeshAgent NMA;
    public GameObject EndPath;
    public int currentPathNode = 1;
    public int childNumber = 0;
    private int health = 10;

        // Use this for initialization
    void Start ()
    {
        childNumber = Random.Range(0, 3);
        EndPath = GameObject.Find("PathWNodes (" + currentPathNode + ")");
        NMA.SetDestination(EndPath.transform.GetChild(childNumber).transform.position);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log("num children:"+EndPath.transform.childCount);
        childNumber = Random.Range(0, 3);
        //Debug.Log(childNumber);

        if (Vector3.Distance(transform.position, NMA.destination) <= 0.5f)
        {
            currentPathNode++;
            EndPath = GameObject.Find("PathWNodes (" + currentPathNode + ")");
            if (EndPath)
            {
                NMA.SetDestination(EndPath.transform.GetChild(childNumber).transform.position);
            }
            else
            {
                GameObject.Find("GameManager").GetComponent<GameController>().reduceHealth();
                Destroy(this.gameObject);
            }
        }
    }

    ///   <summary>
    ///   Decreases health by input amount.
    ///   </summary>
    ///<param name="damage">Amount to decrease health by.
    ///</param>
    void damageHealth(int damage)
    {
        health -= damage;
    }
 
}
