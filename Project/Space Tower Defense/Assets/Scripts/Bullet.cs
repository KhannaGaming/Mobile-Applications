using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Transform target;
    public float speed = 10f;
    public float damage = 2f;
    public GameObject impactEffect;
    public GameObject launchEffect;
    public GameObject trailEffect;
    public HealthManager targetHealth;

    public bool isLaser = false;
    public void Chase(Transform _target)
    {
        //instantiate sposion effect
        target = _target;
        //targetHealth = target.gameObject;
    }
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!isLaser)
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 dir = target.position - transform.position;
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = look;
            float distanceThisFrame = speed * Time.deltaTime;

            //if (dir.magnitude <= distanceThisFrame)
            //{
            //    HitTarget();
            //    return;
            //}

            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        }
        else if (isLaser)
        {
            target.gameObject.SendMessage("TakeDamage", damage/100);
        }
    }

    void HitTarget()
    {

       // GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
       // Destroy(effectIns,2f);
        
        //target.gameObject.HealthManager(2);
        //Destroy(target.gameObject);
        target.gameObject.SendMessage("damageHealth", damage);
        Destroy(gameObject);
    }

    void FaceDirection()
    {
       

    }
    void MoveBullet()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == target.tag)
        {
         Debug.Log("Hit");
            HitTarget();
           // other.GetComponent<EnemyController>().damageHealth(damage);
        }
    }
}
