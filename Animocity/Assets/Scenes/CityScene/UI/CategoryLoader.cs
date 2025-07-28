using Animocity.Cities;
using Animocity.UI;
using BlueprintSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CategoryLoader : MonoBehaviour
{
    public static BuildingBlueprint Selected { get; private set; }
    public Transform ButtonPrefab;
    public BuildingBlueprintSelector buildingSelector;

    private Button lastClicked;

    // Start is called before the first frame update
    void Awake()
    {
        DataLoader.OnDataLoaded += this.LoadAllCategories;
        DataLoader.OnDataCleared += this.Clear;
    }

    void OnDestroy()
    {
        DataLoader.OnDataLoaded -= this.LoadAllCategories;
        DataLoader.OnDataCleared -= this.Clear;
    }

    private void LoadAllCategories(PlayerProfile profile, DataLoader.LoadStatus status)
    {
        print($"Trigged LoadAllcategories!");
        foreach (var cat in BlueprintDatabase<BuildingCategoryBlueprint>.FetchAll())
        {
            print($"Category {cat.label} loading!");
            var t = Transform.Instantiate(ButtonPrefab, this.transform);
            var btn = t.GetComponent<Button>();
            var img = t.GetChild(0).GetComponent<Image>();

            img.sprite = cat.GetSprite();
            img.color = cat.categoryColour;

            btn.onClick.AddListener(() =>
            {
                if(lastClicked == btn)
                {
                    buildingSelector.Clear();
                    lastClicked = null;
                }
                else 
                {
                    lastClicked = btn;
                    buildingSelector.transform.position = t.position + new Vector3(-50, 50, 0);
                    buildingSelector.DisplayCategory(cat);
                }
            });
        }
    }

    private void Clear(PlayerProfile profile, DataLoader.LoadStatus status)
    {
        foreach (Transform t in this.transform)
        {
            GameObject.Destroy(t.gameObject, 0.05f);
        }
    }
}
