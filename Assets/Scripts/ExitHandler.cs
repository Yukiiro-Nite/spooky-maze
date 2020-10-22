using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ExitHandler : MonoBehaviour {
  private MapManager mapManager;
  private AudioSource audio;
  private bool exiting = false;
  public float TransitionTime = 3.0f;
  public string NextScene;
  void Start() {
    mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
    audio = GetComponent<AudioSource>();
  }

  public void DoExit() {
    if(!exiting) {
      exiting = true;
      audio.Play();
      StartCoroutine(TransitionScene(TransitionTime, NextScene));
    }
  }
 
  private IEnumerator TransitionScene(float seconds, string nextScene) {
      yield return new WaitForSeconds(seconds);
      exiting = false;
      SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
      // SteamVR_LoadLevel.Begin(nextScene);
  }
}
