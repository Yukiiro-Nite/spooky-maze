using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInteractions : MonoBehaviour
{
    void Start() {}

    public void LoadNextScene() {
      SceneManager.LoadScene("CaveScene", LoadSceneMode.Single);
    }
}
