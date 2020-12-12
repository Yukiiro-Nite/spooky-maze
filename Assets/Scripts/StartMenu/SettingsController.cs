using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    void Start() {}

    public void SetLevelLength(float length) { SetLevelLength((int) length); }
    public void SetLevelLength(int length) { Settings.LevelLength = length; }

    public void SetHasGuardian(bool flag) { Settings.HasGuardian = flag; }

    public void SetIsHardcore(bool flag) { Settings.IsHardcore = flag; }

    public void SetIsVR(bool flag) { Settings.IsVR = flag; }
}
