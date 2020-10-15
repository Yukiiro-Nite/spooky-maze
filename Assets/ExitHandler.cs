using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ExitHandler : MonoBehaviour {
  private MapManager mapManager;
  private AudioSource audio;
  private bool exiting = false;
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
      Debug.Log("Creating new maze");
      DoExit();
    }
  }

  private void DoExit() {
    exiting = true;
    audio.Play();
    StartCoroutine(Wait(3.0f));
  }
 
  private IEnumerator Wait(float seconds) {
      yield return new WaitForSeconds(seconds);
      mapManager.InitializeMaze("cave");

      exiting = false;
  }
}
