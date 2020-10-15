using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepController : MonoBehaviour {
  public float stepDistance = 0.75f; // close to average step length in meters.
  public float straddleDistance = 0.25f;
  private Vector3 lastStep;
  private int foot = 0; // 0 is right foot, 1 is left foot
  private AudioSource audio;
  void Start() {
    lastStep = Camera.main.transform.position;
    lastStep.y = 0;
    audio = GetComponent<AudioSource>();
  }

  void Update() {
    Vector3 currentPosition = Camera.main.transform.position;
    currentPosition.y = 0;
    float dist = Vector3.Distance(lastStep, currentPosition);

    if(dist >= stepDistance) {
      lastStep = currentPosition;
      
      Vector3 stepLocation = PlayerRelative(Vector3.forward) * (stepDistance / 2.0f);
      Vector3 rightFoot = PlayerRelative(Vector3.right) * (straddleDistance / 2.0f);
      Vector3 leftFoot = PlayerRelative(Vector3.left) * (straddleDistance / 2.0f);
      
      audio.volume = Random.Range(0.6f, 0.8f);
      audio.pitch = Random.Range(0.8f, 1.2f);

      if(foot == 0) {
        foot = 1;
        transform.position = currentPosition + stepLocation + rightFoot;
        audio.Play();
      } else {
        foot  = 0;
        transform.position = currentPosition + stepLocation + leftFoot;
        audio.Play();
      }
    }
  }

  private Vector3 PlayerRelative(Vector3 vec) {
    return Vector3.ProjectOnPlane(
      Camera.main.transform.TransformDirection(vec),
      Vector3.up
    );
  }
}
