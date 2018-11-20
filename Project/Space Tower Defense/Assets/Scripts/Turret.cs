﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    public Transform target;

    [Header("Attributes")]
    public float range = 50f;
    public float rotateSpeed = 10f;

    [Header("Use Bullets")]
    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public GameObject bulletPrefab;

    [Header("Use Laser")]
    public bool useLaser = false;
    public LineRenderer lineRenderer;

    [Header("Use Electricity")]
    public float electricityDelay = 1.0f;
    private float electricityCountdown = 0f;


    [Header("Unity Setup")]
    public string enemyTag = "Enemy";
    public Transform turretHead;
    public bool useElectric = false;
    public Transform[] firePoints;
    public bool fireFromAnimation = false;


    
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
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


    void Shoot(int counter)
    {
       // Debug.Log("Shoot");

        GameObject bulletGo = null;

        GameObject laserGo = null;

        bulletGo = Instantiate(bulletPrefab, firePoints[counter].position, firePoints[counter].rotation);   //casting as a game object to store
        Bullet bullet = bulletGo.GetComponent<Bullet>();
        
        if (bullet != null)
            bullet.Chase(target);

        



    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            animator.SetBool("ifInRange", false);
            if (useLaser || useElectric)
            {
                if (lineRenderer.enabled)
                    lineRenderer.enabled = false;
            }
            return;
        }
        else
        {
            animator.SetBool("ifInRange", true);
        }

        LockOnTarget();

        if (useLaser)
            Laser();
        else if (useElectric)
            Electric();


    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void LockOnTarget()
    {
        ///Target lock
        Vector3 dir = target.position - transform.position; // postion of target minus current position to find directions
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = lookRotation.eulerAngles;
        //Vector3 rotation = Quaternion.Lerp(turretHead.rotation, lookRotation, rotateSpeed * Time.deltaTime).eulerAngles;
        turretHead.rotation = Quaternion.Euler(0f, rotation.y - 90, 0f);
    }

    void Fire()
    {
        if (fireCountdown <= 0f)
        {
            int currentFirePoint = 0;
            while (currentFirePoint < firePoints.Length)
            {
                Shoot(currentFirePoint);
                //have delay inbetween series of fires 
                currentFirePoint++;
            }
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }



   

    void Laser()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePoints[0].position);
        lineRenderer.SetPosition(1, target.position);

    }

    void Electric()
    {
        lineRenderer.enabled = true;

        if (electricityCountdown >= electricityDelay)
        {
            
            target.gameObject.GetComponent<EnemyController>().damageHealth(1.0f);
            electricityCountdown = 0f;
            
        }
            electricityCountdown += Time.deltaTime;
       
        //lightning.StartPosition = firePoints.position;
        //lightning.EndPosition = target.position;
    }
}

