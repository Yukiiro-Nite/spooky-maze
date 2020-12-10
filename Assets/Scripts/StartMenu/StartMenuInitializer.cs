using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuInitializer : MonoBehaviour
{
    public Slider levelLengthSlider;
    public Toggle hasGuardianToggle;
    public Toggle isHardcoreToggle;

    void Start()
    {
        levelLengthSlider.value = Settings.LevelLength;
        hasGuardianToggle.isOn = Settings.HasGuardian;
        isHardcoreToggle.isOn = Settings.IsHardcore;        
    }
}
