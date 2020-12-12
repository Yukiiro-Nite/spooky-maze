using UnityEngine;

public class OnVrEnable : MonoBehaviour
{
    public bool isVR;
    void Start()
    {
      bool isActive = (isVR && Settings.IsVR)
        || (!isVR && !Settings.IsVR);
      gameObject.SetActive(isActive);
    }
}
