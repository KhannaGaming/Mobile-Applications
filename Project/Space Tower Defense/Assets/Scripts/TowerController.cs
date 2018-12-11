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
    byte Max_Upgrade_Level = 4;
    public List<GameObject> Buttons_;

    private Transform Spawned_Tower;
    public GameObject TowerParent;

    public GameObject goldController;
    //private int currentGold = 0;
    public List<GameObject> StandardTurrets;
    private void Start()
    {
        goldController = GameObject.Find("GoldText");
        updateGold();
    }

    public void Buttons()
    {
        foreach (GameObject btn in Buttons_)
        {
            if (Upgrade_Level <= Max_Upgrade_Level)
            {
                if (Upgrade_Level == 0 && btn.tag != "Upgrade")
                    btn.SetActive(!btn.activeInHierarchy);
                else if (Upgrade_Level > 0 && Upgrade_Level < 3 && btn.tag == "Upgrade")
                    btn.SetActive(!btn.activeInHierarchy);
                else if (Upgrade_Level == Max_Upgrade_Level && btn.name == "Delete")
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
        updateGold();
        if (obj.tag != "Upgrade" && Current_Gold >= Prices[Upgrade_Level])
        {
            Spawned_Tower = Instantiate(obj, Vector3.zero, Quaternion.identity, TowerParent.transform).transform;
            Spawned_Tower.localPosition = Vector3.zero;
            Debug.Log("Purchased: " + obj.name + " for " + Prices[Upgrade_Level] + " gold");
            Current_Gold -= Prices[Upgrade_Level];
            goldController.GetComponent<GoldController>().ChangeGoldAmount(-Prices[Upgrade_Level]);
            ++Upgrade_Level;
            Debug.Log("New Gold amount: " + Current_Gold);
            HideButtons();
            // Perform graphical updates on tower to show creation via Spawned_Tower.transform..etc and take off gold
        }
        else if (obj.name == "Upgrade" && Current_Gold >= Prices[Upgrade_Level])
        {
            if (Upgrade_Level < Max_Upgrade_Level)
            {
                Destroy(Spawned_Tower.gameObject);
                Spawned_Tower = Instantiate(StandardTurrets[Upgrade_Level], Vector3.zero, Quaternion.identity, TowerParent.transform).transform;
                Spawned_Tower.localPosition = Vector3.zero;
                Debug.Log(StandardTurrets[Upgrade_Level].name);
                Debug.Log("Upgraded: " + obj.name + " for " + Prices[Upgrade_Level] + " gold");
                Current_Gold -= Prices[Upgrade_Level];
                goldController.GetComponent<GoldController>().ChangeGoldAmount(-Prices[Upgrade_Level]);
                Debug.Log("New Gold amount: " + Current_Gold);
                ++Upgrade_Level;
                if (Upgrade_Level == Max_Upgrade_Level)
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
            //When Deleteing add half the amount of its current upgrade price back to players current gold
            Current_Gold += (Prices[Upgrade_Level-1]/2);
            goldController.GetComponent<GoldController>().ChangeGoldAmount((Prices[Upgrade_Level-1]/2));
            Upgrade_Level = 0;
            HideButtons();
        }
    }
    void updateGold()
    {
        Current_Gold = goldController.GetComponent<GoldController>().GetGoldAmount();
    }
}
