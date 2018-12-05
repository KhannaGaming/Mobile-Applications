using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    public List<GameObject> MenuItems;

    public void Open(string Menu_Item_Name)
    {
        Debug.Log(Menu_Item_Name);
        foreach (GameObject item in MenuItems)
        {
            if (item.name == Menu_Item_Name)
            {
                item.SetActive(true);
            }
            else
                item.SetActive(false);
        }
    }

    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
