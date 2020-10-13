using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PickUpHand : MonoBehaviour {
  public float pickupDistance = 0.2f;
  bool handClosed = false;
  public LayerMask pickupLayer;
  public SteamVR_Input_Sources handSource;
  Rigidbody grabTarget;
  
  void FixedUpdate() {
    handClosed = SteamVR_Actions.default_GrabPinch.GetState(handSource);
    
    if (!handClosed) {
      Collider[] colliders = Physics.OverlapSphere(transform.position, pickupDistance, pickupLayer);
      grabTarget = colliders.Length > 0
        ? colliders[0].transform.root.GetComponent<Rigidbody>()
        : null;
    } else {
      if (grabTarget) {
        grabTarget.velocity = (transform.position - grabTarget.transform.position) / Time.fixedDeltaTime;
        grabTarget.maxAngularVelocity = 20;
        Quaternion deltaRot = transform.rotation * Quaternion.Inverse(grabTarget.transform.rotation);
        Vector3 eulerRot = new Vector3(
          Mathf.DeltaAngle(0, deltaRot.eulerAngles.x),
          Mathf.DeltaAngle(0, deltaRot.eulerAngles.y),
          Mathf.DeltaAngle(0, deltaRot.eulerAngles.z)
        );
        eulerRot *= 0.95f;
        eulerRot *= Mathf.Deg2Rad;
        grabTarget.angularVelocity = eulerRot / Time.fixedDeltaTime;
      }
    }
  }
}
