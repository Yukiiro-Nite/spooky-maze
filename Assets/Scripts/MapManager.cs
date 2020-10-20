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
  public float CellPadding = 0f;
  public int RenderDepth = 5;
  private Maze maze;
  private static readonly Dictionary<string, string> nextType = new Dictionary<string, string> {
    {"cave", "sewer"},
    {"sewer", "cave"},
  };
  private Dictionary<string, MazeConfig> mazeConf;
  // Start is called before the first frame update
  void Start() {
    mazeConf = new Dictionary<string, MazeConfig> {
      {"cave", new MazeConfig(Width, Height, 2.5f, 5f, 1f, 1f, 5)},
      {"sewer", new MazeConfig(Width, Height, 2.5f, 2.5f, 2.25f, 1f, 7)}
    };
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
    this.maze = MazeGenerator.generate(Width, Height, GetNextMazeType());
    MazeConfig config = CurrentConfig();
    WallBuilder wallBuilder = GetBuilder(maze, config);
    wallBuilder.BuildMaze(maze.getCell(0, 0), config.RenderDepth);

    GameObject player = GameObject.Find("Player");
    GameObject exit = GameObject.Find("Exit");

    player.transform.position = new Vector3(
      maze.start.x * config.CellSize,
      0f,
      maze.start.y * config.CellSize
    );
    exit.transform.position = new Vector3(
      maze.end.x * config.CellSize,
      config.CeilingHeight,
      maze.end.y * config.CellSize
    );
  }

  public void UpdateMaze(int x, int y) {
    ClearMaze();

    MazeConfig config = CurrentConfig();
    WallBuilder wallBuilder = GetBuilder(maze, config);
    wallBuilder.BuildMaze(maze.getCell(x, y), config.RenderDepth);
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

  public MazeConfig CurrentConfig() {
    MazeConfig defaultConfig = new MazeConfig(Width, Height, CeilingHeight, CellSize, MinWidth, CellPadding, RenderDepth);
    return mazeConf.ContainsKey(maze.type)
      ? mazeConf[maze.type]
      : defaultConfig;
  }

  public WallBuilder GetBuilder(Maze maze, MazeConfig config) {
    switch(maze.type) {
      case "cave": return new WallBuilder(maze, config);
      case "sewer": return new SewerBuilder(maze, config);
      default: return new WallBuilder(maze, config);
    }
  }
}
