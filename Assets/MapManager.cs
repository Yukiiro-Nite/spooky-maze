using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
  public int Width = 50;
  public int Height = 50;
  public float CeilingHeight = 2.5f;
  public float CellSize = 3f;
  public float MinWidth = 1f;
  private Maze maze;
  // Start is called before the first frame update
  void Start() {
    this.maze = InitializeMaze("cave");
  }

  // Update is called once per frame
  void Update() {
    
  }

  public Maze InitializeMaze(string type) {
    /*
      1. generate maze
      2. add maze to scene
      3. move player to maze start
    */
    Maze maze = MazeGenerator.generate(Width, Height, type);

    Debug.Log("Finished generating maze.");
    Debug.Log(maze.start);
    Debug.Log(maze.end);

    Debug.Log("Creating Wall Set.");
    WallBuilder wallBuilder = new WallBuilder(maze, CellSize, MinWidth, CeilingHeight);
    wallBuilder.BuildMaze(maze.getCell(0, 0), 5);

    Debug.Log("Testing neigborhood fn...");
    Debug.Log(maze.Neighborhood(0, 0, 1).Count);

    return maze;
  }
}
