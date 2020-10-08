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
    // Maze maze = new Maze(1, 2, type);
    // maze.openWall(0, 0, "south");

    Debug.Log("Finished generating maze.");
    Debug.Log(maze.start);
    Debug.Log(maze.end);
    // Cell startCell = maze.getCell(0, 0);
    // Cell endCell = maze.getCell(0, 1);

    // Debug.Log("Start Cell Walls: north: " + startCell.north + ", east: " + startCell.east + ", south: " + startCell.south + ", west: " + startCell.west);
    // Debug.Log("End Cell Walls: north: " + endCell.north + ", east: " + endCell.east + ", south: " + endCell.south + ", west: " + endCell.west);

    Debug.Log("Creating Wall Set.");
    WallBuilder wallBuilder = new WallBuilder(maze, CellSize, MinWidth, CeilingHeight);
    // List<GameObject> quads = wallBuilder.Quads();

    HashSet<Cell> neighborhood = maze.Neighborhood(0, 0, 5);
    HashSet<Wall> nearbyWalls = wallBuilder.GetWalls(neighborhood);

    foreach(Wall wall in nearbyWalls) {
      wallBuilder.CreateQuad(wall);
    }

    Debug.Log("Testing neigborhood fn...");
    Debug.Log(maze.Neighborhood(0, 0, 1).Count);

    return maze;
  }
}
