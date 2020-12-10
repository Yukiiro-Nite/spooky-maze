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
  public GameObject Player;
  public GameObject Exit;
  public Maze maze;
  private WallBuilder builder;
  private Dictionary<GameObject, Vector3> objectsToPlace = new Dictionary<GameObject, Vector3>();
  private static readonly Dictionary<string, string> nextType = new Dictionary<string, string> {
    {"cave", "sewer"},
    {"sewer", "cave"},
  };
  // Start is called before the first frame update
  void Start() {
    Width = Settings.LevelLength;
    Height = Settings.LevelLength;
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
    builder = GetBuilder(maze, CellSize, MinWidth, CeilingHeight, CellPadding);
    // builder.BuildMaze(maze.getCell(0, 0), RenderDepth);
    builder.BuildMaze();
    NavGrid navGrid = GameObject.Find("Navigation").GetComponent<NavGrid>();
    navGrid.CreateGrid();

    PlaceObject(Player, maze.start, 0f);
    PlaceObject(Exit, maze.end, CeilingHeight);
  }

  public void UpdateMaze(int x, int y) {
    ClearMaze();

    builder = GetBuilder(maze, CellSize, MinWidth, CeilingHeight, CellPadding);
    builder.BuildMaze(maze.getCell(x, y), RenderDepth);
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

  public WallBuilder GetBuilder(Maze maze, float CellSize, float MinWidth, float CeilingHeight, float CellPadding) {
    switch(maze.type) {
      case "cave": return new WallBuilder(maze, CellSize, MinWidth, CeilingHeight, CellPadding);
      case "sewer": return new SewerBuilder(maze, CellSize, MinWidth, CeilingHeight, CellPadding);
      case "office": return new OfficeBuilder(maze, CellSize, MinWidth, CeilingHeight, CellPadding);
      default: return new WallBuilder(maze, CellSize, MinWidth, CeilingHeight, CellPadding);
    }
  }

  public void PlaceObject(GameObject obj, Vector2 pos, float height) {
    if(builder == null) {
      objectsToPlace.Add(obj, new Vector3(pos.x, height, pos.y));
    } else {
      builder.PlaceObject(obj, pos, height);
      foreach(KeyValuePair<GameObject, Vector3> pair in objectsToPlace) {
        builder.PlaceObject(pair.Key, new Vector2(pair.Value.x, pair.Value.z), pair.Value.y);
      }
      objectsToPlace.Clear();
    }
  }

  public Vector2 GetGridPosition(GameObject obj) {
    float size = CellSize + CellPadding;
    return new Vector2(
      (float)Math.Ceiling((obj.transform.position.x - size /  2.0) / size),
      (float)Math.Ceiling((obj.transform.position.z - size /  2.0) / size)
    );
  }

  public Vector2 GetWorldPosition(Vector2 gridPosition)
  {
    float size = CellSize + CellPadding;
    return gridPosition * size;
  }
}
