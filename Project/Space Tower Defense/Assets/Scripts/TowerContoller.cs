using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerContoller : MonoBehaviour
{
    // Used to reference the tower prefabs
    public GameObject standardTowerPrefab;
    public GameObject laserTowerPrefab;
    public GameObject rocketTowerPrefab;
    public GameObject electricityTowerPrefab;
    public GameObject sniperTowerPrefab;

    // Used to reference the tower button brefabs
    public GameObject standardTowerButtonPrefab;
    public GameObject laserTowerButtonPrefab;
    public GameObject rocketTowerButtonPrefab;
    public GameObject electricityTowerButtonPrefab;
    public GameObject sniperTowerButtonPrefab;

    // Use this for initialization
    void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void spawnButtons()
    {

    }

    public void CreateStandardTower()
    {
        Instantiate(standardTowerPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("TowerSlot").transform);
    }

    public void CreateLaserTower()
    {
        Instantiate(laserTowerPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("TowerSlot").transform);
    }

    public void CreateRocketTower()
    {
        Instantiate(rocketTowerPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("TowerSlot").transform);
    }

    public void CreateElectricityTower()
    {
        Instantiate(electricityTowerPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("TowerSlot").transform);
    }

    public void CreateSniperTower()
    {
        Instantiate(sniperTowerPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("TowerSlot").transform);
    }
}
