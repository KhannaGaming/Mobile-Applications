using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    public Transform target;
    public float speed = 10f;
    public float damage = 0.1f;
    public GameObject impactEffect;
    public GameObject launchEffect;
    public GameObject trailEffect;
    public HealthManager targetHealth;

    public void Chase(Transform _target)
    {
        //instantiate sposion effect
        target = _target;
        //targetHealth = target.gameObject;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        target.gameObject.SendMessage("TakeDamage", 0.1f);
	}
}
