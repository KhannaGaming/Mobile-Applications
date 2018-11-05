using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    public Transform target;

    [Header("Attributes")]
    public float range = 50f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public float rotateSpeed = 10f;


    [Header("Unity Setup")]
    public string enemyTag = "Enemy";
    public Transform turretHead;

    public GameObject bulletPrefab;
    public Transform firePoints;


    // Use this for initialization
    void Start ()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
	}
	
    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach(GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if(distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
            target = nearestEnemy.transform;
        else
            target = null;

    }

    void Shoot()
    {
        Debug.Log("Shoot");

        GameObject bulletGo = null;
        int counter = 0;
        
            bulletGo = (GameObject)Instantiate(bulletPrefab, firePoints.position, firePoints.rotation);   //casting as a game object to store
            Bullet bullet = bulletGo.GetComponent<Bullet>();

            if (bullet != null)
                bullet.Chase(target);
       


    }

    // Update is called once per frame
    void Update ()
    {
        if (target == null)
            return;
        ///Target lock
        Vector3 dir = target.position - transform.position; // postion of target minus current position to find directions
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = lookRotation.eulerAngles;
        //Vector3 rotation = Quaternion.Lerp(turretHead.rotation, lookRotation, rotateSpeed * Time.deltaTime).eulerAngles;
        turretHead.rotation = Quaternion.Euler (0f, rotation.y -90, 0f);
        
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
