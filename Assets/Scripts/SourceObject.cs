using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SourceObject : MonoBehaviour
{

    public string sourceName;
    public TextMeshProUGUI label;


    public void UpdateInfo()
    {
        label.text = name;
    }
}
