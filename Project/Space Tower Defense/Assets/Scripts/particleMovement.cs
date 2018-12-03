using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleMovement : MonoBehaviour {
    
    ParticleSystem ps;
    ParticleSystem.Particle[] pslist;
    public float speed = 0.1f;
    public float m_Drift = 0.01f;

    public int currentPathNode = 1;
    public int childNumber = 0;
    public GameObject EndPath;
    public GameObject StartPath;
    // Use this for initialization
    void Start () {
        ps = GetComponent<ParticleSystem>();
        childNumber = Random.Range(0, 3);
        EndPath = GameObject.Find("PathWNodes (" + currentPathNode + ")");

    }

    // Update is called once per frame
    void Update() {

        ParticleSystem.MainModule main = this.ps.main;
        if (this.pslist == null || this.pslist.Length < main.maxParticles)
        {

            this.pslist = new ParticleSystem.Particle[main.maxParticles];

        }
        int count = this.ps.GetParticles(this.pslist);
       
        this.ps.SetParticles(this.pslist, count);
    }
}
