using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepController : MonoBehaviour {
  public List<AudioClip> audioClips;
  private AudioSource[] sources;
  public float frequency = 0.04f;
  public float radius = 10.0f;
  public float minVolume = 0.5f;
  public float maxVolume = 1.0f;
  public float minPitch = 0.8f;
  public float maxPitch = 1.2f;
  public float stepDistance = 0.75f; // close to average step length in meters.
  public float straddleDistance = 0.25f;
  private Vector3 lastStep;
  private int foot = 0; // 0 is right foot, 1 is left foot
  void Start() {
    lastStep = Camera.main.transform.position;
    lastStep.y = 0;
    sources = new AudioSource[audioClips.Count];
    for(int i=0; i<audioClips.Count; i++) {
        sources[i] = gameObject.AddComponent<AudioSource>();
        sources[i].spatialBlend = 1.0f;
        sources[i].clip = audioClips[i];
    }
  }

  void Update() {
    Vector3 currentPosition = Camera.main.transform.position;
    currentPosition.y = 0;
    float dist = Vector3.Distance(lastStep, currentPosition);

    if(dist >= stepDistance) {
      lastStep = currentPosition;
      AudioSource audio = sources[Random.Range(0, sources.Length)];
      
      Vector3 stepLocation = PlayerRelative(Vector3.forward) * (stepDistance / 2.0f);
      Vector3 rightFoot = PlayerRelative(Vector3.right) * (straddleDistance / 2.0f);
      Vector3 leftFoot = PlayerRelative(Vector3.left) * (straddleDistance / 2.0f);
      
      audio.volume = Random.Range(minVolume, maxVolume);
      audio.pitch = Random.Range(minPitch, maxPitch);

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
