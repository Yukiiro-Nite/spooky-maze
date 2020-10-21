using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMaze : MonoBehaviour {
  private Vector2 gridPosition;
  private MapManager mapManager;
  // Start is called before the first frame update
  void Start() {
    mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
    gridPosition = GetGridPosition();
  }

  // Update is called once per frame
  void Update() {
    Vector2 newPos = GetGridPosition();
    if(newPos.x != gridPosition.x || newPos.y != gridPosition.y) {
      gridPosition = newPos;
      mapManager.UpdateMaze((int) gridPosition.x, (int) gridPosition.y);
    }
  }

  public Vector2 GetGridPosition() {
    return new Vector2(
      (float)Math.Ceiling((Camera.main.transform.position.x - mapManager.CellSize /  2.0) / mapManager.CellSize),
      (float)Math.Ceiling((Camera.main.transform.position.z - mapManager.CellSize /  2.0) / mapManager.CellSize)
    );
  }
}
