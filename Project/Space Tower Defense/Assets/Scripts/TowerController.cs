using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class TowerController : MonoBehaviour
{

    private int[] Prices = new int[4] { 10, 50, 100, 200 }; //Needs to be removed
    public int Current_Gold = 10; //Needs to be removed

    byte Upgrade_Level = 0;

    public List<GameObject> Buttons_;

    private Transform Spawned_Tower;
    public GameObject TowerParent;
    public void Buttons()
    {
        foreach (GameObject btn in Buttons_)
        {
            if (Upgrade_Level <= 3)
            {
                if (Upgrade_Level == 0 && btn.tag != "Upgrade")
                    btn.SetActive(!btn.activeInHierarchy);
                else if (Upgrade_Level > 0 && Upgrade_Level < 3 && btn.tag == "Upgrade")
                    btn.SetActive(!btn.activeInHierarchy);
                else if (Upgrade_Level == 3 && btn.name == "Delete")
                    btn.SetActive(!btn.activeInHierarchy);
            }
        }
    }
    public void HideButtons()
    {
        foreach (GameObject btn in Buttons_)
            btn.SetActive(false);
    }
    public void Action(GameObject obj)
    {
        if (obj.tag != "Upgrade" && Current_Gold >= Prices[Upgrade_Level])
        {
            Spawned_Tower = Instantiate(obj, Vector3.zero, Quaternion.identity, TowerParent.transform).transform;
            Spawned_Tower.localPosition = Vector3.zero;
            ++Upgrade_Level;
            Debug.Log("Purchased: " + obj.name + " for " + Prices[Upgrade_Level] + " gold");
            Current_Gold -= Prices[Upgrade_Level];
            Debug.Log("New Gold amount: " + Current_Gold);
            HideButtons();
            // Perform graphical updates on tower to show creation via Spawned_Tower.transform..etc and take off gold
        }
        else if (obj.name == "Upgrade" && Current_Gold >= Prices[Upgrade_Level])
        {
            if (Upgrade_Level < 3)
            {
                ++Upgrade_Level;
                Debug.Log("Upgraded: " + obj.name + " for " + Prices[Upgrade_Level] + " gold");
                Current_Gold -= Prices[Upgrade_Level];
                Debug.Log("New Gold amount: " + Current_Gold);
                if (Upgrade_Level == 3)
                {
                    HideButtons();
                }
                // Perform graphical updates on tower to show upgrade via Spawned_Tower.transform..etc and take off gold
            }
        }
        else if (obj.name == "Delete")
        {
            Destroy(Spawned_Tower.gameObject);
            obj.SetActive(true);
            Upgrade_Level = 0;
            HideButtons();
        }
    }
}
