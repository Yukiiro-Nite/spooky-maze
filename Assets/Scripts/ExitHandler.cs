using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
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

  private void OnHandHoverBegin(Hand hand) {
    hand.ShowGrabHint();
  }

  private void OnHandHoverEnd(Hand hand) {
    hand.HideGrabHint();
  }

  private void HandHoverUpdate(Hand hand) {
    GrabTypes grabType = hand.GetGrabStarting();

    if(grabType != GrabTypes.None && !exiting) {
      DoExit();
    }
  }

  private void DoExit() {
    exiting = true;
    audio.Play();
    StartCoroutine(TransitionScene(TransitionTime, NextScene));
  }
 
  private IEnumerator TransitionScene(float seconds, string nextScene) {
      yield return new WaitForSeconds(seconds);
      exiting = false;
      // SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
      SteamVR_LoadLevel.Begin(nextScene);
  }
}
