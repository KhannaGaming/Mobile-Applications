using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Transform target;
    public float speed = 10f;
    public GameObject impactEffect;
    public void Chase(Transform _target)
    {
        //instantiate sposion effect
        target = _target;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        //FaceDirection();
        Vector3 dir = target.position - transform.position;
        Quaternion look = Quaternion.LookRotation(dir);
        transform.rotation = look;
        float distanceThisFrame = speed * Time.deltaTime;

        if(dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
	}

    void HitTarget()
    {

        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 2f);

        //Destroy(target.gameObject);

        Destroy(gameObject);
        //Debug.Log("Hit");
    }

    void FaceDirection()
    {
        //Vector3 direction = (target.position - gameObject.transform.position).normalized;

    }


}
