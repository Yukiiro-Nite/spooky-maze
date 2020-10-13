using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchFlicker : MonoBehaviour {
  private Color color;
  private Light light;
  public float sparkRate = 0.04f;
  public float intensityDecay = 0.002f;
  void Start() {
    light = this.GetComponent<Light>();
    color = FireColor();

    light.color = color;
  }

  void Update() {
    if(Random.value <= sparkRate) {
      color = FireColor();
      light.color = color;
      light.intensity = 1.0f;
    } else {
      light.intensity -= intensityDecay;
    }
  }

  Color FireColor() {
    return Random.ColorHSV(0.09f, 0.125f, 0.60f, 0.60f, 0.75f, 0.75f);
  }
}
