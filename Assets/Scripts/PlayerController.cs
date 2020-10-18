using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerController : MonoBehaviour {
  public SteamVR_Action_Vector2 touchpadInput;
  private CapsuleCollider playerCollider;
  void Start() {
    playerCollider = GetComponent<CapsuleCollider>();
  }

  void FixedUpdate() {
    Vector3 dir = Player.instance.hmdTransform.TransformDirection(new Vector3(touchpadInput.axis.x, 0, touchpadInput.axis.y));
    transform.position += Vector3.ProjectOnPlane(Time.deltaTime * dir * 2.0f, Vector3.up);

    float distanceFromFloor = Vector3.Dot(Camera.main.transform.localPosition, Vector3.up);
    playerCollider.height = Mathf.Max(playerCollider.radius, distanceFromFloor + 0.2f);
    playerCollider.center = Camera.main.transform.localPosition - 0.5f * distanceFromFloor * Vector3.up;
  }
}
