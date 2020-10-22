using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : MonoBehaviour {
  public float speed = 2f;
  public XRNode InputSrc;
  public LayerMask groundLayer;
  private XRRig rig;
  private CharacterController player;
  private Vector2 inputAxis;
  private float fallingSpeed = 0f;
  void Start() {
    player = GetComponent<CharacterController>();
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
    player.Move(dir * Time.fixedDeltaTime * speed);
    
    fallingSpeed = IsGrounded()
      ? 0
      : fallingSpeed += Physics.gravity.y * Time.fixedDeltaTime;
    
    player.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
  }

  void FollowHeadset() {
    player.height = rig.cameraInRigSpaceHeight + 0.2f;
    Vector3 center = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
    player.center = new Vector3(center.x, player.height / 2f + player.skinWidth, center.z);
  }

  bool IsGrounded() {
    Vector3 rayStart = transform.TransformPoint(player.center);
    float rayLength = player.center.y + 0.01f;
    bool hit = Physics.SphereCast(rayStart, player.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
    return hit;
  }
}
