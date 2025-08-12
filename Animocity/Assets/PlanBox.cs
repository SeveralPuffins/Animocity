using Animocity.Cities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanBox : MonoBehaviour
{
    public TMP_Text costs;
    public Button finishBuildingButton;

    private Building building;


    public void SetBuilding(Building building)
    {
        this.building = building;

        string costsStr = "";

        foreach(var res in building.Blue.resourceCosts.Keys)
        {
            costsStr += $"{building.Blue.resourceCosts[res]} {res.DisplayName}\n";
        }

        costs.text = costsStr;

        finishBuildingButton.onClick.AddListener(() =>
                        {
                            if (building.TryCommitBuild(false))
                            {
                                //Some way of selecting the new building here!
                            }
                        });
    }

    private void Update()
    {
        if (building.CanAfford())
        {
            costs.color = Color.green;
            finishBuildingButton.interactable = true;  
        }
        else
        {
            costs.color = Color.yellow;
            finishBuildingButton.interactable = false;
        }
    }
}
