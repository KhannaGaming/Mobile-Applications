using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : MonoBehaviour {

    public Transform target;

    [Header("Attributes")]
    public float range = 2f;
    public float range2 = 2f;
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

    float distanceToEnemy = 0f;

    public enum ShootStyle { First, Last, Strongest, Weakest};
    public ShootStyle shootStyle;

    public List<GameObject> EnemiesInRange = null;



    private Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("UpdateTarget", 0f, 0.1f);
        transform.localPosition = Vector3.zero;
    }

    void UpdateTarget()
    {

        switch(shootStyle)
        {
            case ShootStyle.First:
                FirstEnemy();
                break;
            case ShootStyle.Last:
                LastEnemy();
                break;
            case ShootStyle.Strongest:
                StrongestEnemy();
                break;
            case ShootStyle.Weakest:
                WeakestEnemy();
                break;

        }

    }


    void FirstEnemy()
    {
        if(EnemiesInRange.Count >0)
        target = EnemiesInRange[0].transform;
    }

    void LastEnemy()
    {
        if (EnemiesInRange.Count > 0)
            target = EnemiesInRange[EnemiesInRange.Count-1].transform;
    }

    void StrongestEnemy()
    {
        if (target == null && EnemiesInRange.Count>0)
        {
            target = EnemiesInRange[0].transform;
        }
        if (target != null)
        {
            for (int i = 0; i < EnemiesInRange.Count; i++)
            {
                if (EnemiesInRange[i].GetComponent<EnemyController>().health >= target.GetComponent<EnemyController>().health)
                {
                    target = EnemiesInRange[i].transform;
                }

            }
        }
    }

    void WeakestEnemy()
    {
        if(EnemiesInRange.Count == 0)
        {
            target = null;
        }
        if (target == null && EnemiesInRange.Count > 0)
        {
            target = EnemiesInRange[0].transform;
        }
        if (target != null)
        {
            for (int i = 0; i < EnemiesInRange.Count; i++)
            {
                if (EnemiesInRange[i].GetComponent<EnemyController>().health <= target.GetComponent<EnemyController>().health)
                {
                    target = EnemiesInRange[i].transform;
                }

            }
        }
    }

    void Shoot(int counter)
    {
        // Debug.Log("Shoot");
       
            GameObject bulletGo = null;



            bulletGo = Instantiate(bulletPrefab, firePoints[counter].position, firePoints[counter].rotation, this.transform);   //casting as a game object to store
            Bullet bullet = bulletGo.GetComponent<Bullet>();

            if (bullet != null)
               bullet.Chase(target);
        
        



    }

    private void FixedUpdate()
    {
            
        
    }
    // Update is called once per frame
    void Update()
    {
        RemoveDeadEnemies();
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


            electricityCountdown = 0f;
            
        }
            electricityCountdown += Time.deltaTime;
       
        //lightning.StartPosition = firePoints.position;
        //lightning.EndPosition = target.position;
    }



    private void OnTriggerEnter(Collider other)
    {
        RemoveDeadEnemies();
        if (other.tag == "Enemy" && !EnemiesInRange.Contains(other.gameObject))
        {
            EnemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RemoveDeadEnemies();
        if(other.tag == "Enemy" && EnemiesInRange.Contains(other.gameObject))
        {
            EnemiesInRange.Remove(other.gameObject);
        }
    }
    public void RemoveDeadEnemies()
    {
        for (int i = 0; i < EnemiesInRange.Count; i++)
        {
            if(EnemiesInRange[i] == null)
            {
                EnemiesInRange.RemoveAt(i);
            }
        }
    }

}

