using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
  public string defaultType = "cave";
  public int Width = 50;
  public int Height = 50;
  public float CeilingHeight = 2.5f;
  public float CellSize = 3f;
  public float MinWidth = 1f;
  private Maze maze;
  private static readonly Dictionary<string, string> nextType = new Dictionary<string, string> {
    {"cave", "sewer"},
    {"sewer", "cave"},
  };
  // Start is called before the first frame update
  void Start() {
    InitializeMaze();
  }

  // Update is called once per frame
  void Update() {
    
  }

  public void InitializeMaze() {
    ClearMaze();
    /*
      1. generate maze
      2. add maze to scene
      3. move player to maze start
    */
    Maze maze = MazeGenerator.generate(Width, Height, GetNextMazeType());
    WallBuilder wallBuilder = GetBuilder(maze, CellSize, MinWidth, CeilingHeight);
    wallBuilder.BuildMaze(maze.getCell(0, 0), 5);

    GameObject player = GameObject.Find("Player");
    GameObject exit = GameObject.Find("Exit");

    player.transform.position = new Vector3(maze.start.x * CellSize, 0f, maze.start.y * CellSize);
    exit.transform.position = new Vector3(maze.end.x * CellSize, CeilingHeight, maze.end.y * CellSize);

    this.maze = maze;
  }

  public void UpdateMaze(int x, int y) {
    ClearMaze();

    WallBuilder wallBuilder = GetBuilder(maze, CellSize, MinWidth, CeilingHeight);
    wallBuilder.BuildMaze(maze.getCell(x, y), 5);
  }

  public void ClearMaze() {
    GameObject.Destroy(GameObject.Find("floors"));
    GameObject.Destroy(GameObject.Find("walls"));
    GameObject.Destroy(GameObject.Find("ceilings"));
    GameObject.Destroy(GameObject.Find("channels"));
  }

  public string GetNextMazeType() {
    return this.maze == null
      ? defaultType
      : nextType[this.maze.type];
  }

  public WallBuilder GetBuilder(Maze maze, float CellSize, float MinWidth, float CeilingHeight) {
    switch(maze.type) {
      case "cave": return new WallBuilder(maze, CellSize, MinWidth, CeilingHeight);
      case "sewer": return new SewerBuilder(maze, CellSize, MinWidth, CeilingHeight);
      default: return new WallBuilder(maze, CellSize, MinWidth, CeilingHeight);
    }
  }
}
