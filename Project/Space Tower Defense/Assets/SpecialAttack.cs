using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : MonoBehaviour {


    public enum Special { Nebula, Beam, MOAB };
    public Special special;
    public float NebulaTime = 5.0f;
    public float Nebulafactor = 0.5f;
  
    public float beamDamage = 4.0f;

    private float beamDuration = 3.0f;

    // Use this for initialization
    void Start ()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        switch (special)
        {
            case Special.Nebula:
                    
                break;
            case Special.Beam:

                break;
            case Special.MOAB:

                break;


        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
            other.gameObject.GetComponent<EnemyController>().changeSpeed(Nebulafactor);

        other.gameObject.GetComponent<EnemyController>().damageHealth(beamDamage);
    }

    // Update is called once per frame
    void Update ()
    {

        Destroy(gameObject, NebulaTime);
        Destroy(gameObject, beamDamage);
    }
}
