using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuilder {
  private Maze maze;
  private float CellSize;
  private float MinWidth;
  private float CeilingHeight;

  private static readonly Vector2 nwDirection = new Vector2(-1, -1);
  private static readonly Vector2 neDirection = new Vector2(1, -1);
  private static readonly Vector2 seDirection = new Vector2(1, 1);
  private static readonly Vector2 swDirection = new Vector2(-1, 1);
  private static readonly string[] directions = new string[4] {
    "north",
    "east",
    "south",
    "west"
  };
  public WallBuilder(Maze maze, float CellSize, float MinWidth, float CeilingHeight) {
    this.maze = maze;
    this.CellSize = CellSize;
    this.MinWidth = MinWidth;
    this.CeilingHeight = CeilingHeight;
  }

  public List<GameObject> Quads() {
    HashSet<Wall> walls = GetWalls();
    List<GameObject> gameObjects = new List<GameObject>();

    foreach(Wall currentWall in walls) {
      gameObjects.Add(CreateQuad(currentWall));
    }

    return gameObjects;
  }

  public GameObject CreateQuad(Wall wall) {
    GameObject gameObject = new GameObject("Wall");
    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
    meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

    Mesh mesh = new Mesh();

    Vector3[] vertices = new Vector3[4] {
      new Vector3(wall.P1.x, 0, wall.P1.y),
      new Vector3(wall.P1.x, CeilingHeight, wall.P1.y),
      new Vector3(wall.P2.x, CeilingHeight, wall.P2.y),
      new Vector3(wall.P2.x, 0, wall.P2.y)
    };
    mesh.vertices = vertices;

    int[] tris = new int[6] {
      0, 2, 1,
      0, 3, 2
    };
    mesh.triangles = tris;

    Vector3 normal = new Plane(vertices[0], vertices[2], vertices[1]).normal;
    Vector3[] normals = new Vector3[4] {
      normal,
      normal,
      normal,
      normal
    };
    mesh.normals = normals;

    // double check these later, Y coord might be upside down.
    // I'm not sure if UV coords start at top left or bottom left.
    Vector2[] uv = new Vector2[4] {
      new Vector2(0, 0),
      new Vector2(0, 1),
      new Vector2(1, 1),
      new Vector2(1, 0)
    };
    mesh.uv = uv;

    meshFilter.mesh = mesh;

    return gameObject;
  }

  public HashSet<Wall> GetWalls() {
    HashSet<Wall> walls = new HashSet<Wall>();

    for (int y = 0; y < maze.height; y++) {
      for (int x = 0; x < maze.width; x++) {
        HashSet<Wall> cellWalls = GetWallsForCell(x, y);
        walls.UnionWith(cellWalls);
      }
    }

    return walls;
  }

  public HashSet<Wall> GetWalls(HashSet<Cell> cells) {
    HashSet<Wall> walls = new HashSet<Wall>();

    foreach (Cell cell in cells) {
      HashSet<Wall> cellWalls = GetWallsForCell(cell);
      walls.UnionWith(cellWalls);
    }

    return walls;
  }

  private HashSet<Wall> GetWallsForCell(Cell cell) {
    return GetWallsForCell(cell.x, cell.y);
  }

  private HashSet<Wall> GetWallsForCell(int x, int y) {
    HashSet<Wall> cellWalls = new HashSet<Wall>();

    Cell cell = maze.getCell(x, y);
    Vector2 positionOffset = new Vector2(x * CellSize, y * CellSize);

    Cell northNeighbor = maze.getCell(x, y-1);
    Vector2 northOffset = new Vector2(x * CellSize, (y-1) * CellSize);

    Cell eastNeighbor = maze.getCell(x+1, y);
    Vector2 eastOffset = new Vector2((x+1) * CellSize, y * CellSize);

    Cell southNeighbor = maze.getCell(x, y+1);
    Vector2 southOffset = new Vector2(x * CellSize, (y+1) * CellSize);

    Cell westNeighbor = maze.getCell(x-1, y);
    Vector2 westOffset = new Vector2((x-1) * CellSize, y * CellSize);

    if(northNeighbor != null && cell.north && northNeighbor.south) {
      // setup walls between cell.nw && north.sw, cell.ne && north.se
      cellWalls.Add(new Wall(
        worldPosition(cell.nwOffset, nwDirection, positionOffset),
        worldPosition(northNeighbor.swOffset, swDirection, northOffset)
      ));
      cellWalls.Add(new Wall(
        worldPosition(cell.neOffset, neDirection, positionOffset),
        worldPosition(northNeighbor.seOffset, seDirection, northOffset)
      ));

    } else {
      // setup walls between cell.nw && cell.ne
      cellWalls.Add(new Wall(
        worldPosition(cell.nwOffset, nwDirection, positionOffset),
        worldPosition(cell.neOffset, neDirection, positionOffset)
      ));
    }

    if(eastNeighbor != null && cell.east && eastNeighbor.west) {
      cellWalls.Add(new Wall(
        worldPosition(cell.neOffset, neDirection, positionOffset),
        worldPosition(eastNeighbor.nwOffset, nwDirection, eastOffset)
      ));
      cellWalls.Add(new Wall(
        worldPosition(eastNeighbor.swOffset, swDirection, eastOffset),
        worldPosition(cell.seOffset, seDirection, positionOffset)
      ));

    } else {
      cellWalls.Add(new Wall(
        worldPosition(cell.neOffset, neDirection, positionOffset),
        worldPosition(cell.seOffset, seDirection, positionOffset)
      ));
    }

    if(southNeighbor != null && cell.south && southNeighbor.north) {
      cellWalls.Add(new Wall(
        worldPosition(cell.seOffset, seDirection, positionOffset),
        worldPosition(southNeighbor.neOffset, neDirection, southOffset)
      ));
      cellWalls.Add(new Wall(
        worldPosition(southNeighbor.nwOffset, nwDirection, southOffset),
        worldPosition(cell.swOffset, swDirection, positionOffset)
      ));
    } else {
      cellWalls.Add(new Wall(
        worldPosition(cell.seOffset, seDirection, positionOffset),
        worldPosition(cell.swOffset, swDirection, positionOffset)
      ));
    }

    if(westNeighbor != null && cell.west && westNeighbor.east) {
      cellWalls.Add(new Wall(
        worldPosition(cell.swOffset, swDirection, positionOffset),
        worldPosition(westNeighbor.seOffset, seDirection, westOffset)
      ));
      cellWalls.Add(new Wall(
        worldPosition(cell.nwOffset, nwDirection, positionOffset),
        worldPosition(westNeighbor.neOffset, neDirection, westOffset)
      ));
    } else {
      cellWalls.Add(new Wall(
        worldPosition(cell.swOffset, swDirection, positionOffset),
        worldPosition(cell.nwOffset, nwDirection, positionOffset)
      ));
    }

    return cellWalls;
  }

  private Vector2 worldPosition(
    Vector2 innerOffset,
    Vector2 direction,
    Vector2 position
  ) {
    float wiggleSpace = (CellSize - MinWidth) / 2.0f;
    float minWidthOffset = MinWidth / 2.0f;
    return innerOffset * direction * wiggleSpace
      + (position + direction * minWidthOffset);
  }

  public void BuildMaze(Cell cell, int depth) {
    Dictionary<Cell, int> visited = new Dictionary<Cell, int>();
    Dictionary<string, List<Mesh>> meshes = new Dictionary<string, List<Mesh>>();
    meshes.Add("floor", new List<Mesh>());
    meshes.Add("wall", new List<Mesh>());
    meshes.Add("ceiling", new List<Mesh>());
    

    BuildCell(cell, depth, visited, meshes);
    CreateObject("floors", meshes["floor"], new Material(Shader.Find("Standard")));
    CreateObject("walls", meshes["wall"], new Material(Shader.Find("Standard")));
    CreateObject("ceilings", meshes["ceiling"], new Material(Shader.Find("Standard")));
  }

  private void CreateObject(string name, List<Mesh> meshes, Material material) {
    GameObject gameObject = new GameObject(name);
    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
    meshRenderer.sharedMaterial = material;

    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
    Mesh mesh = new Mesh();

    mesh.CombineMeshes(GetCombineInstances(meshes));

    meshFilter.mesh = mesh;
  }

  private CombineInstance[] GetCombineInstances(List<Mesh> meshes) {
    CombineInstance[] combineInstances = new CombineInstance[meshes.Count];

    for(int i=0; i < meshes.Count; i++) {
      combineInstances[i].subMeshIndex = 0; // might not need to set this.
      combineInstances[i].mesh = meshes[i];
      combineInstances[i].transform = Matrix4x4.identity;
    }

    return combineInstances;
  }

  private void BuildCell(Cell cell, int depth, Dictionary<Cell, int> visited, Dictionary<string, List<Mesh>> meshes) {
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


  private Mesh QuadFromPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
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
  private void BuildCellMesh(Cell cell, Dictionary<string, List<Mesh>> meshes) {
    // Debug.Log("Building cell (" + cell.x + ", " + cell.y + ")");
    Vector2 cellPosition = new Vector2(cell.x * CellSize, cell.y * CellSize);
    Vector2 nwPoint = worldPosition(cell.nwOffset, nwDirection, cellPosition);
    Vector2 nePoint = worldPosition(cell.neOffset, neDirection, cellPosition);
    Vector2 sePoint = worldPosition(cell.seOffset, seDirection, cellPosition);
    Vector2 swPoint = worldPosition(cell.swOffset, swDirection, cellPosition);

    meshes["floor"].Add(Floor(swPoint, nwPoint, nePoint, sePoint));
    meshes["ceiling"].Add(Ceiling(swPoint, nwPoint, nePoint, sePoint));
  }

  private void BuildPathMesh(Cell from, Cell to, string dir, Dictionary<string, List<Mesh>> meshes) {
    // Debug.Log("Building " + dir + " path from (" + from.x + ", " + from.y + ")");
    Vector2 fromPos = new Vector2(from.x * CellSize, from.y * CellSize);
    Vector2 nwFrom = worldPosition(from.nwOffset, nwDirection, fromPos);
    Vector2 neFrom = worldPosition(from.neOffset, neDirection, fromPos);
    Vector2 seFrom = worldPosition(from.seOffset, seDirection, fromPos);
    Vector2 swFrom = worldPosition(from.swOffset, swDirection, fromPos);

    Vector2 toPos = new Vector2(to.x * CellSize, to.y * CellSize);
    Vector2 nwTo = worldPosition(to.nwOffset, nwDirection, toPos);
    Vector2 neTo = worldPosition(to.neOffset, neDirection, toPos);
    Vector2 seTo = worldPosition(to.seOffset, seDirection, toPos);
    Vector2 swTo = worldPosition(to.swOffset, swDirection, toPos);

    Mesh leftWall = new Mesh(),
         rightWall = new Mesh(),
         floor = new Mesh(),
         ceiling = new Mesh();

    if(dir == "north") {
      leftWall = LeftWall(nwFrom, swTo);
      rightWall = RightWall(neFrom, seTo);
      floor = Floor(nwFrom, swTo, seTo, neFrom);
      ceiling = Ceiling(nwFrom, swTo, seTo, neFrom);
    } else if(dir == "east") {
      leftWall = LeftWall(neFrom, nwTo);
      rightWall = RightWall(seFrom, swTo);
      floor = Floor(neFrom, nwTo, swTo, seFrom);
      ceiling = Ceiling(neFrom, nwTo, swTo, seFrom);
    } else if(dir == "south") {
      leftWall = LeftWall(seFrom, neTo);
      rightWall = RightWall(swFrom, nwTo);
      floor = Floor(seFrom, neTo, nwTo, swFrom);
      ceiling = Ceiling(seFrom, neTo, nwTo, swFrom);
    } else if(dir == "west") {
      leftWall = LeftWall(swFrom, seTo);
      rightWall = RightWall(nwFrom, neTo);
      floor = Floor(swFrom, seTo, neTo, nwFrom);
      ceiling = Ceiling(swFrom, seTo, neTo, nwFrom);
    }

    meshes["wall"].Add(leftWall);
    meshes["wall"].Add(rightWall);
    meshes["floor"].Add(floor);
    meshes["ceiling"].Add(ceiling);
  }

  private void BuildWallMesh(Cell from, string dir, Dictionary<string, List<Mesh>> meshes) {
    // Debug.Log("Building " + dir + " wall for (" + from.x + ", " + from.y + ")");
    Vector2 fromPos = new Vector2(from.x * CellSize, from.y * CellSize);
    Vector2 nwFrom = worldPosition(from.nwOffset, nwDirection, fromPos);
    Vector2 neFrom = worldPosition(from.neOffset, neDirection, fromPos);
    Vector2 seFrom = worldPosition(from.seOffset, seDirection, fromPos);
    Vector2 swFrom = worldPosition(from.swOffset, swDirection, fromPos);

    Mesh wall = new Mesh();

    if(dir == "north") {
      wall = LeftWall(nwFrom, neFrom);
    } else if(dir == "east") {
      wall = LeftWall(neFrom, seFrom);
    } else if(dir == "south") {
      wall = LeftWall(seFrom, swFrom);
    } else if(dir == "west") {
      wall = LeftWall(swFrom, nwFrom);
    }

    meshes["wall"].Add(wall);
  }

  private Mesh LeftWall(Vector2 nearLeft, Vector2 farLeft) {
    return QuadFromPoints(
      new Vector3(nearLeft.x, CeilingHeight, nearLeft.y),
      new Vector3(farLeft.x, CeilingHeight, farLeft.y),
      new Vector3(farLeft.x, 0, farLeft.y),
      new Vector3(nearLeft.x, 0, nearLeft.y)
    );
  }
  private Mesh RightWall(Vector2 nearRight, Vector2 farRight) {
    return QuadFromPoints(
      new Vector3(farRight.x, CeilingHeight, farRight.y),
      new Vector3(nearRight.x, CeilingHeight, nearRight.y),
      new Vector3(nearRight.x, 0, nearRight.y),
      new Vector3(farRight.x, 0, farRight.y)
    );
  }

  private Mesh Floor(Vector2 nearLeft, Vector2 farLeft, Vector2 farRight, Vector2 nearRight) {
    return QuadFromPoints(
      new Vector3(farLeft.x, 0, farLeft.y),
      new Vector3(farRight.x, 0, farRight.y),
      new Vector3(nearRight.x, 0, nearRight.y),
      new Vector3(nearLeft.x, 0, nearLeft.y)
    );
  }

  private Mesh Ceiling(Vector2 nearLeft, Vector2 farLeft, Vector2 farRight, Vector2 nearRight) {
    return QuadFromPoints(
      new Vector3(nearLeft.x, CeilingHeight, nearLeft.y),
      new Vector3(nearRight.x, CeilingHeight, nearRight.y),
      new Vector3(farRight.x, CeilingHeight, farRight.y),
      new Vector3(farLeft.x, CeilingHeight, farLeft.y)
    );
  }
}