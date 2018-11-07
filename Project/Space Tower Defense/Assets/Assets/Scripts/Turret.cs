using DigitalRuby.LightningBolt;
using System.Collections;
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

    public LightningBoltScript lightning;

    [Header("Unity Setup")]
    public string enemyTag = "Enemy";
    public Transform turretHead;
    public bool useElectric = false;
    public Transform firePoints;



    // Use this for initialization
    void Start()
    {
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
    void Update()
    {
        if (target == null)
        {
            if (lineRenderer.enabled)
                lineRenderer.enabled = false;
            return;
        }

        LockOnTarget();

        if (useLaser)
            Laser();
        else if (useElectric)
            Electric();
        else
            Fire();


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
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Laser()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePoints.position);
        lineRenderer.SetPosition(1, target.position);
    }

    void Electric()
    {
        lineRenderer.enabled = true;
        lightning.StartObject = firePoints.gameObject;
        lightning.EndObject = target.gameObject;

        //lightning.StartPosition = firePoints.position;
        //lightning.EndPosition = target.position;
    }
}

