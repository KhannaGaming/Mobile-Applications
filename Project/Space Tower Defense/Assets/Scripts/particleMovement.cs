using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleMovement : MonoBehaviour {
    
    ParticleSystem ps;
    ParticleSystem.Particle[] pslist;
    public float speed = 1.0f;
    public float m_Drift = 0.01f;

    // Use this for initialization
    void Start () {
        ps = GetComponent<ParticleSystem>();
	}

    // Update is called once per frame
    void Update() {
        if (pslist == null || pslist.Length < ps.main.maxParticles)
        {
            pslist = new ParticleSystem.Particle[ps.main.maxParticles];
        }

      int count = ps.GetParticles(pslist);

        for (int i = 0; i < count; i++)
        {
            //pslist[i].velocity += Vector3.up * m_Drift;
            pslist[i].position =  Vector3.MoveTowards(pslist[i].position, Vector3.one, speed);
           
        }
        ps.SetParticles(pslist, count);
    }
}
