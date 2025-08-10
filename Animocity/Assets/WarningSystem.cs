using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningSystem : MonoBehaviour
{
    public static WarningSystem Current;
    private Dictionary<string, Image> warnings;
    private void Awake()
    {
        Current = this;
        warnings = new();
    }

}
