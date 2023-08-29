using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StoreScreen : MonoBehaviour
{
    public List<GameObject> StoreItems { get; private set; }
    public StoreItem SelectedItem { get; private set; }
    
    void Start()
    {
        StoreItems = GameObject.FindGameObjectsWithTag("StoreItem").ToList();
        MenuController.Instance.GameCursor = GameObject.Find("GameCursor").GetComponent<Image>();
    }

    void Update()
    {

    }

    public void SelectItem(StoreItem item)
    {
        if (SelectedItem != null)
            SelectedItem.Deselect();

        SelectedItem = item;
        SelectedItem.Select();
    }
}
