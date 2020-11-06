using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuilder {
  protected Maze maze;
  protected float CellSize;
  protected float MinWidth;
  protected float CeilingHeight;
  protected float CellPadding;

  protected static readonly Vector2 nwDirection = new Vector2(-1, -1);
  protected static readonly Vector2 neDirection = new Vector2(1, -1);
  protected static readonly Vector2 seDirection = new Vector2(1, 1);
  protected static readonly Vector2 swDirection = new Vector2(-1, 1);
  protected static readonly string[] directions = new string[4] {
    "north",
    "east",
    "south",
    "west"
  };
  public WallBuilder(Maze maze, float CellSize, float MinWidth, float CeilingHeight, float CellPadding) {
    this.maze = maze;
    this.CellSize = CellSize;
    this.MinWidth = MinWidth;
    this.CeilingHeight = CeilingHeight;
    this.CellPadding = CellPadding;
  }

  protected Vector2 worldPosition(
    Vector2 innerOffset,
    Vector2 direction,
    Vector2 position
  ) {
    float wiggleSpace = (CellSize - MinWidth) / 2.0f;
    float minWidthOffset = MinWidth / 2.0f;
    return innerOffset * direction * wiggleSpace
      + (position + direction * minWidthOffset);
  }

  public virtual void BuildMaze(Cell cell, int depth) {
    Dictionary<Cell, int> visited = new Dictionary<Cell, int>();
    Dictionary<string, List<Mesh>> meshes = new Dictionary<string, List<Mesh>>();
    meshes.Add("floor", new List<Mesh>());
    meshes.Add("wall", new List<Mesh>());
    meshes.Add("ceiling", new List<Mesh>());
    
    BuildCell(cell, depth, visited, meshes);
    CreateObject("floors", meshes["floor"], maze.type);
    CreateObject("walls", meshes["wall"], maze.type);
    CreateObject("ceilings", meshes["ceiling"], maze.type);
  }

  public virtual void BuildMaze() {
    Cell startCell = maze.getCell(Vector2.zero);
    int depth = maze.width * maze.height;

    BuildMaze(startCell, depth);
  }

  protected void CreateObject(string name, List<Mesh> meshes, string zoneType) {
    Material material = MaterialMap.Get(zoneType, name);
    GameObject gameObject = new GameObject(name);
    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
    meshRenderer.sharedMaterial = material;

    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
    Mesh mesh = new Mesh();

    mesh.CombineMeshes(GetCombineInstances(meshes));

    meshFilter.mesh = mesh;

    MeshCollider collider = gameObject.AddComponent<MeshCollider>();
    Rigidbody body = gameObject.AddComponent<Rigidbody>();

    body.isKinematic = true;

    if(name == "walls") {
      gameObject.layer = 10;
    } else if (name == "floors" || name == "channels") {
      gameObject.layer = 11;
    }

  }

  protected CombineInstance[] GetCombineInstances(List<Mesh> meshes) {
    CombineInstance[] combineInstances = new CombineInstance[meshes.Count];

    for(int i=0; i < meshes.Count; i++) {
      combineInstances[i].subMeshIndex = 0; // might not need to set this.
      combineInstances[i].mesh = meshes[i];
      combineInstances[i].transform = Matrix4x4.identity;
    }

    return combineInstances;
  }

  protected void BuildCell(Cell cell, int depth, Dictionary<Cell, int> visited, Dictionary<string, List<Mesh>> meshes) {
    visited.Add(cell, depth);
    BuildCellMesh(cell, meshes);

    foreach(string dir in directions) {
      Cell neighbor = maze.getNeighbor(cell, dir);
      bool isConnected = neighbor != null
        && maze.IsConnected(cell, neighbor, dir);
      bool allowedToBuild = neighbor != null
        && (
          !visited.ContainsKey(neighbor)
          || visited.ContainsKey(neighbor)
          && visited[neighbor] < depth
        );
      if(isConnected) {
        if(allowedToBuild) {
          BuildPathMesh(cell, neighbor, dir, meshes);
          if(depth-1 >= 0) {
            BuildCell(neighbor, depth-1, visited, meshes);
          }
        } else {
          // if a connected neighbor has a higher depth,
          // it will take care of building the path later.
        }
      } else {
        BuildWallMesh(cell, dir, meshes);
      }
    }
  }

  protected Mesh QuadFromPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
    Mesh mesh = new Mesh();
    mesh.vertices = new Vector3[4] {p0, p1, p2, p3};

    int[] tris = new int[6] {
      0, 2, 1,
      0, 3, 2
    };
    mesh.triangles = tris;

    mesh.RecalculateNormals();

    Vector2[] uv = new Vector2[4] {
      new Vector2(0, 0),
      new Vector2(0, 1),
      new Vector2(1, 1),
      new Vector2(1, 0)
    };
    mesh.uv = uv;

    return mesh;
  }
  protected virtual void BuildCellMesh(Cell cell, Dictionary<string, List<Mesh>> meshes) {
    // Debug.Log("Building cell (" + cell.x + ", " + cell.y + ")");
    Vector2 cellPosition = new Vector2(cell.x, cell.y) * (CellSize + CellPadding);
    Vector2 nwPoint = worldPosition(cell.nwOffset, nwDirection, cellPosition);
    Vector2 nePoint = worldPosition(cell.neOffset, neDirection, cellPosition);
    Vector2 sePoint = worldPosition(cell.seOffset, seDirection, cellPosition);
    Vector2 swPoint = worldPosition(cell.swOffset, swDirection, cellPosition);

    Floor(swPoint, nwPoint, nePoint, sePoint, meshes, GetDirections(cell, "north"));
    Ceiling(swPoint, nwPoint, nePoint, sePoint, meshes, GetDirections(cell, "north"));
  }

  protected virtual void BuildPathMesh(Cell from, Cell to, string dir, Dictionary<string, List<Mesh>> meshes) {
    // Debug.Log("Building " + dir + " path from (" + from.x + ", " + from.y + ")");
    Vector2 fromPos = new Vector2(from.x, from.y) * (CellSize + CellPadding);
    Vector2 nwFrom = worldPosition(from.nwOffset, nwDirection, fromPos);
    Vector2 neFrom = worldPosition(from.neOffset, neDirection, fromPos);
    Vector2 seFrom = worldPosition(from.seOffset, seDirection, fromPos);
    Vector2 swFrom = worldPosition(from.swOffset, swDirection, fromPos);

    Vector2 toPos = new Vector2(to.x, to.y) * (CellSize + CellPadding);
    Vector2 nwTo = worldPosition(to.nwOffset, nwDirection, toPos);
    Vector2 neTo = worldPosition(to.neOffset, neDirection, toPos);
    Vector2 seTo = worldPosition(to.seOffset, seDirection, toPos);
    Vector2 swTo = worldPosition(to.swOffset, swDirection, toPos);

    if(dir == "north") {
      LeftWall(nwFrom, swTo, meshes);
      RightWall(neFrom, seTo, meshes);
      Floor(nwFrom, swTo, seTo, neFrom, meshes, GetPathDirections());
      Ceiling(nwFrom, swTo, seTo, neFrom, meshes, GetPathDirections());
    } else if(dir == "east") {
      LeftWall(neFrom, nwTo, meshes);
      RightWall(seFrom, swTo, meshes);
      Floor(neFrom, nwTo, swTo, seFrom, meshes, GetPathDirections());
      Ceiling(neFrom, nwTo, swTo, seFrom, meshes, GetPathDirections());
    } else if(dir == "south") {
      LeftWall(seFrom, neTo, meshes);
      RightWall(swFrom, nwTo, meshes);
      Floor(seFrom, neTo, nwTo, swFrom, meshes, GetPathDirections());
      Ceiling(seFrom, neTo, nwTo, swFrom, meshes, GetPathDirections());
    } else if(dir == "west") {
      LeftWall(swFrom, seTo, meshes);
      RightWall(nwFrom, neTo, meshes);
      Floor(swFrom, seTo, neTo, nwFrom, meshes, GetPathDirections());
      Ceiling(swFrom, seTo, neTo, nwFrom, meshes, GetPathDirections());
    }
  }

  protected virtual void BuildWallMesh(Cell from, string dir, Dictionary<string, List<Mesh>> meshes) {
    // Debug.Log("Building " + dir + " wall for (" + from.x + ", " + from.y + ")");
    Vector2 fromPos = new Vector2(from.x, from.y) * (CellSize + CellPadding);;
    Vector2 nwFrom = worldPosition(from.nwOffset, nwDirection, fromPos);
    Vector2 neFrom = worldPosition(from.neOffset, neDirection, fromPos);
    Vector2 seFrom = worldPosition(from.seOffset, seDirection, fromPos);
    Vector2 swFrom = worldPosition(from.swOffset, swDirection, fromPos);

    if(dir == "north") {
      LeftWall(nwFrom, neFrom, meshes);
    } else if(dir == "east") {
      LeftWall(neFrom, seFrom, meshes);
    } else if(dir == "south") {
      LeftWall(seFrom, swFrom, meshes);
    } else if(dir == "west") {
      LeftWall(swFrom, nwFrom, meshes);
    }
  }

  protected virtual void LeftWall(Vector2 nearLeft, Vector2 farLeft, Dictionary<string, List<Mesh>> meshes) {
    meshes["wall"].Add(QuadFromPoints(
      new Vector3(nearLeft.x, CeilingHeight, nearLeft.y),
      new Vector3(farLeft.x, CeilingHeight, farLeft.y),
      new Vector3(farLeft.x, 0, farLeft.y),
      new Vector3(nearLeft.x, 0, nearLeft.y)
    ));
  }
  protected virtual void RightWall(Vector2 nearRight, Vector2 farRight, Dictionary<string, List<Mesh>> meshes) {
    meshes["wall"].Add(QuadFromPoints(
      new Vector3(farRight.x, CeilingHeight, farRight.y),
      new Vector3(nearRight.x, CeilingHeight, nearRight.y),
      new Vector3(nearRight.x, 0, nearRight.y),
      new Vector3(farRight.x, 0, farRight.y)
    ));
  }

  protected virtual void Floor(Vector2 nearLeft, Vector2 farLeft, Vector2 farRight, Vector2 nearRight, Dictionary<string, List<Mesh>> meshes, Dictionary<string, bool> dir) {
    meshes["floor"].Add(QuadFromPoints(
      new Vector3(farLeft.x, 0, farLeft.y),
      new Vector3(farRight.x, 0, farRight.y),
      new Vector3(nearRight.x, 0, nearRight.y),
      new Vector3(nearLeft.x, 0, nearLeft.y)
    ));
  }

  protected virtual void Ceiling(Vector2 nearLeft, Vector2 farLeft, Vector2 farRight, Vector2 nearRight, Dictionary<string, List<Mesh>> meshes, Dictionary<string, bool> dir) {
    meshes["ceiling"].Add(QuadFromPoints(
      new Vector3(nearLeft.x, CeilingHeight, nearLeft.y),
      new Vector3(nearRight.x, CeilingHeight, nearRight.y),
      new Vector3(farRight.x, CeilingHeight, farRight.y),
      new Vector3(farLeft.x, CeilingHeight, farLeft.y)
    ));
  }

  protected Dictionary<string, bool> GetDirections(Cell cell, string direction) {
    switch(direction) {
      case "north": return new Dictionary<string, bool> {
        {"front", cell.north},
        {"right", cell.east},
        {"back", cell.south},
        {"left", cell.west}
      };
      case "east": return new Dictionary<string, bool> {
        {"front", cell.east},
        {"right", cell.south},
        {"back", cell.west},
        {"left", cell.north}
      };
      case "south": return new Dictionary<string, bool> {
        {"front", cell.south},
        {"right", cell.west},
        {"back", cell.north},
        {"left", cell.east}
      };
      case "west": return new Dictionary<string, bool> {
        {"front", cell.west},
        {"right", cell.north},
        {"back", cell.east},
        {"left", cell.south}
      };
      default: return new Dictionary<string, bool> {
        {"front", cell.north},
        {"right", cell.east},
        {"back", cell.south},
        {"left", cell.west}
      };
    }
  }

  protected Dictionary<string, bool> GetPathDirections() {
    return new Dictionary<string, bool> {
        {"front", true},
        {"right", false},
        {"back", true},
        {"left", false},
      };
  }

  public void PlaceObject(GameObject obj, Vector2 mazePos, float height) {
    Vector2 worldPos = mazePos * (CellSize + CellPadding);
    obj.transform.position = new Vector3(worldPos.x, height, worldPos.y);
  }
}