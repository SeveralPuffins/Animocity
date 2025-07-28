using Animocity.UI;
using BlueprintSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCategories : MonoBehaviour
{
    public Transform InventoryCategoryButtonPrefab;
    // Start is called before the first frame update
    void Awake()
    {
        DataLoader.OnDataLoaded += this.OnDataLoaded; 
        DataLoader.OnDataCleared += this.OnDataCleared;
    }

    private void OnDestroy()
    {
        DataLoader.OnDataLoaded -= this.OnDataLoaded;
        DataLoader.OnDataCleared -= this.OnDataCleared;
    }


    private void OnDataCleared(PlayerProfile profile, DataLoader.LoadStatus Status)
    {
        foreach(Transform t in this.transform)
        {
            Destroy(t.gameObject, 0.02f);
        }
    }

    private void OnDataLoaded(PlayerProfile profile, DataLoader.LoadStatus Status)
    {
        foreach(var inventoryBlue in BlueprintDatabase<InventoryMenuCategoryBlue>.FetchAll())
        {
            Transform newCategoryButton = Transform.Instantiate(InventoryCategoryButtonPrefab, this.transform);
            var cat = newCategoryButton.GetComponent<InventoryMenuCategory>();
            cat.Initialise(inventoryBlue);
        }
    }

}
