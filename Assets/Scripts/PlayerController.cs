using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : MonoBehaviour {
  public float speed = 2f;
  public XRNode InputSrc;
  private XRRig rig;
  private CapsuleCollider player;
  private Vector2 inputAxis;
  private float fallingSpeed = 0f;
  void Start() {
    player = GetComponent<CapsuleCollider>();
    rig = GetComponent<XRRig>();
  }

  void Update() {
    InputDevice device = InputDevices.GetDeviceAtXRNode(InputSrc);
    device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
  }

  void FixedUpdate() {
    FollowHeadset();
    Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
    Vector3 dir = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
    if(!WillCollide(dir)) {
      transform.position += dir * Time.fixedDeltaTime * speed;
    }
  }

  void FollowHeadset() {
    player.height = rig.cameraInRigSpaceHeight + 0.2f;
    Vector3 localCameraPos = rig.cameraInRigSpacePos;
    player.center = new Vector3(localCameraPos.x, player.height / 2, localCameraPos.z);
  }

  bool WillCollide(Vector3 dir) {
    Vector3 rayOrigin = Camera.main.transform.position - dir.normalized * player.radius;
    LayerMask notPlayer = ~LayerMask.GetMask("Player");
    return Physics.SphereCast(
      rayOrigin,
      0.1f,
      dir,
      out RaycastHit hit,
      player.radius * 2,
      notPlayer
    );
  }
}
