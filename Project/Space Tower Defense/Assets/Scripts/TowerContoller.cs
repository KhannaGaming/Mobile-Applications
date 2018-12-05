using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject TowerSlot;

    public List<GameObject> TowerButtons = new List<GameObject>();

    public void spawnButtons()
    {
        GameObject GO = Instantiate(standardTowerButtonPrefab, Vector3.zero, Quaternion.identity, this.transform);
        GO.transform.localPosition = new Vector3 (-35,0,0);
        GO.GetComponent<Button>().onClick.AddListener(CreateStandardTower);

        GO = Instantiate(laserTowerButtonPrefab, Vector3.zero, Quaternion.identity, this.transform);
        GO.transform.localPosition = new Vector3(-18, 39, 0);
        GO.GetComponent<Button>().onClick.AddListener(CreateLaserTower);

        GO = Instantiate(rocketTowerButtonPrefab, Vector3.zero, Quaternion.identity, this.transform);
        GO.transform.localPosition = new Vector3(21, 39, 0);
        GO.GetComponent<Button>().onClick.AddListener(CreateRocketTower);

        GO = Instantiate(electricityTowerButtonPrefab, Vector3.zero, Quaternion.identity, this.transform);
        GO.transform.localPosition = new Vector3(38, 2, 0);
        GO.GetComponent<Button>().onClick.AddListener(CreateElectricityTower);

        GO = Instantiate(sniperTowerButtonPrefab, Vector3.zero, Quaternion.identity, this.transform);
        GO.transform.localPosition = new Vector3(2, -30, 0);
        GO.GetComponent<Button>().onClick.AddListener(CreateSniperTower);

        foreach (var item in GameObject.FindGameObjectsWithTag("TowerButton"))
        {
            TowerButtons.Add(item);
        }
    }

    public void CreateStandardTower()
    {
        Instantiate(standardTowerPrefab, Vector3.zero, Quaternion.identity, TowerSlot.transform).transform.localPosition = Vector3.zero;
        DestroyButtons();
    }

    public void CreateLaserTower()
    {
        Instantiate(laserTowerPrefab, Vector3.zero, Quaternion.identity, TowerSlot.transform).transform.localPosition = Vector3.zero;
        DestroyButtons();
    }

    public void CreateRocketTower()
    {
        Instantiate(rocketTowerPrefab, Vector3.zero, Quaternion.identity, TowerSlot.transform).transform.localPosition = Vector3.zero;
        DestroyButtons();
    }

    public void CreateElectricityTower()
    {
        Instantiate(electricityTowerPrefab, Vector3.zero, Quaternion.identity, TowerSlot.transform).transform.localPosition = Vector3.zero;
        DestroyButtons();
    }

    public void CreateSniperTower()
    {
        Instantiate(sniperTowerPrefab, Vector3.zero, Quaternion.identity, TowerSlot.transform).transform.localPosition = Vector3.zero;
        DestroyButtons();
    }

    public void DestroyButtons()
    {
        
        for (int i = TowerButtons.Count-1; i >= 0; i--)
        {
            Destroy(TowerButtons[i]);
            TowerButtons.Remove(TowerButtons[i]);
        }

    }
}
