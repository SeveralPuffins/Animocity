using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class InventoryListerOption : MonoBehaviour
{
    public TMP_Text text;
    public Image icon;

    public Func<string> textUpdateFunc;

    private void Update()
    {
        text.text = textUpdateFunc();
    }

    public void SetSprite(Sprite newSprite)
    {
        icon.sprite = newSprite;
    }
}
