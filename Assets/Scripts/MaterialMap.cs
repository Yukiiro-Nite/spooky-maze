using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialMap {
  protected static readonly Dictionary<String, Dictionary<String, Material>> materials = new Dictionary<string, Dictionary<string, Material>> {
    {"cave", new Dictionary<string, Material> {
      {"floors", Resources.Load("CaveFloor") as Material},
      {"walls", Resources.Load("CaveWall") as Material},
      {"ceilings", Resources.Load("CaveWall") as Material}
    }},
    {"sewer", new Dictionary<string, Material> {
      {"floors", Resources.Load("Brick") as Material},
      {"walls", Resources.Load("Brick") as Material},
      {"ceilings", Resources.Load("Brick") as Material},
      {"channels", Resources.Load("SewerWater_2") as Material}
    }},
  };

  public static Material Get(String zoneType, String faceType) {
    return materials[zoneType][faceType];
  }
}