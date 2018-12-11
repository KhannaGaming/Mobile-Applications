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
    public GameObject InstantiateThis;
    public enum TurretType { Standard, Laser, Rocket, Electric, Sniper };
    public TurretType shootStyle;

    public List<GameObject> staticShock = null;
    public GameObject staticBullet = null;
    public int electricBounce = 3;
    public float shockWeaknesFactor = 2.0f;

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



            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        }
        else if (isLaser)
        {
            target.gameObject.SendMessage("TakeDamage", damage/100);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target != null && other.gameObject == target.gameObject)
        {
            Debug.Log("Hit");
            staticShock.Add(other.gameObject);
            HitTarget();
        }
    }


    void HitTarget()
    {
        switch (shootStyle)
        {
            case TurretType.Standard:
                target.gameObject.SendMessage("damageHealth", damage);
                Destroy(gameObject);
                break;
            case TurretType.Electric:
                ThunderStrike(transform.GetChild(0).GetComponent<Shock>().staticShock);
                target.gameObject.SendMessage("damageHealth", damage);
                Destroy(gameObject); 
                break;
            case TurretType.Rocket:
                Debug.Log("Rocket Hit");
                Instantiate(InstantiateThis, this.transform.position, Quaternion.identity);
                Destroy(gameObject);
                break;
            case TurretType.Laser:
                
                break;
            case TurretType.Sniper:
                target.gameObject.SendMessage("damageHealth", damage);
                Destroy(gameObject);
                break;

        }
        
    }

    void FaceDirection()
    {
       

    }
    void MoveBullet()
    {

    }

    public void ThunderStrike (List<GameObject> targets)
    {
        electricBounce += 1;

        for (int i = 0; i < staticShock.Count; i++)
        {
            if (staticShock[i] == null)
                staticShock.RemoveAt(i);
        }

        Debug.Log("Begining lightning Attack");

        if (targets.Count < electricBounce) //if potential bounce targets less than amout of times we can bouce 
            electricBounce = targets.Count; //set to smaller number

        for (int i = 1; i < electricBounce; i++)    //create bullet for each bounce
        {
            GameObject staticGo = null;             
            staticGo = Instantiate(staticBullet, this.transform.position, this.transform.rotation);   //
            Bullet sBullet = staticGo.GetComponent<Bullet>();

            if (sBullet != null)
            {
                
                sBullet.damage = damage / shockWeaknesFactor;
                sBullet.Chase(targets[i].transform);

            }

        }
    }

    public void TestingSend()
    {
        Debug.Log("Begining lightning Attack");
    }

}
