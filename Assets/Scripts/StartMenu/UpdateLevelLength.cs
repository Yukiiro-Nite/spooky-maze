using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateLevelLength : MonoBehaviour
{
    private Text textEl;
    void Start()
    {
        textEl = GetComponent<Text>();
    }

    void Update()
    {
        if(Int32.Parse(textEl.text) != Settings.LevelLength) {
            textEl.text = "" + Settings.LevelLength;
        }
    }
}
