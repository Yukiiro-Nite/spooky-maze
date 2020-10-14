using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ExitHandler : MonoBehaviour {
  private MapManager mapManager;
  void Start() {
    mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
  }

  private void OnHandHoverBegin(Hand hand) {
    hand.ShowGrabHint();
  }

  private void OnHandHoverEnd(Hand hand) {
    hand.HideGrabHint();
  }

  private void HandHoverUpdate(Hand hand) {
    GrabTypes grabType = hand.GetGrabStarting();

    if(grabType != GrabTypes.None) {
      Debug.Log("Creating new maze");
      mapManager.InitializeMaze("cave");
    }
  }
}
