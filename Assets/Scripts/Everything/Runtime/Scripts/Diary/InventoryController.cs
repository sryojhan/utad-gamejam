using UnityEngine;


public class InventoryController : MonoBehaviour
{

    public GameObject[] inventory;

    public void ActivateItem(int i)
    {
        inventory[i].SetActive(true);
    }

    public void UseItem(int i)
    {
        inventory[i].SetActive(false);
    }
   
}
