using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuilder {
  private Maze maze;
  private float CellSize;
  private float MinWidth;
  private float CeilingHeight;
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

    Vector2 nwDirection = new Vector2(-1, -1);
    Vector2 neDirection = new Vector2(1, -1);
    Vector2 seDirection = new Vector2(1, 1);
    Vector2 swDirection = new Vector2(-1, 1);

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
        absoluteWallPosition(cell.nwOffset, nwDirection, positionOffset),
        absoluteWallPosition(northNeighbor.swOffset, swDirection, northOffset)
      ));
      cellWalls.Add(new Wall(
        absoluteWallPosition(cell.neOffset, neDirection, positionOffset),
        absoluteWallPosition(northNeighbor.seOffset, seDirection, northOffset)
      ));

    } else {
      // setup walls between cell.nw && cell.ne
      cellWalls.Add(new Wall(
        absoluteWallPosition(cell.nwOffset, nwDirection, positionOffset),
        absoluteWallPosition(cell.neOffset, neDirection, positionOffset)
      ));
    }

    if(eastNeighbor != null && cell.east && eastNeighbor.west) {
      cellWalls.Add(new Wall(
        absoluteWallPosition(cell.neOffset, neDirection, positionOffset),
        absoluteWallPosition(eastNeighbor.nwOffset, nwDirection, eastOffset)
      ));
      cellWalls.Add(new Wall(
        absoluteWallPosition(eastNeighbor.swOffset, swDirection, eastOffset),
        absoluteWallPosition(cell.seOffset, seDirection, positionOffset)
      ));

    } else {
      cellWalls.Add(new Wall(
        absoluteWallPosition(cell.neOffset, neDirection, positionOffset),
        absoluteWallPosition(cell.seOffset, seDirection, positionOffset)
      ));
    }

    if(southNeighbor != null && cell.south && southNeighbor.north) {
      cellWalls.Add(new Wall(
        absoluteWallPosition(cell.seOffset, seDirection, positionOffset),
        absoluteWallPosition(southNeighbor.neOffset, neDirection, southOffset)
      ));
      cellWalls.Add(new Wall(
        absoluteWallPosition(southNeighbor.nwOffset, nwDirection, southOffset),
        absoluteWallPosition(cell.swOffset, swDirection, positionOffset)
      ));
    } else {
      cellWalls.Add(new Wall(
        absoluteWallPosition(cell.seOffset, seDirection, positionOffset),
        absoluteWallPosition(cell.swOffset, swDirection, positionOffset)
      ));
    }

    if(westNeighbor != null && cell.west && westNeighbor.east) {
      cellWalls.Add(new Wall(
        absoluteWallPosition(cell.swOffset, swDirection, positionOffset),
        absoluteWallPosition(westNeighbor.seOffset, seDirection, westOffset)
      ));
      cellWalls.Add(new Wall(
        absoluteWallPosition(cell.nwOffset, nwDirection, positionOffset),
        absoluteWallPosition(westNeighbor.neOffset, neDirection, westOffset)
      ));
    } else {
      cellWalls.Add(new Wall(
        absoluteWallPosition(cell.swOffset, swDirection, positionOffset),
        absoluteWallPosition(cell.nwOffset, nwDirection, positionOffset)
      ));
    }

    return cellWalls;
  }

  private Vector2 absoluteWallPosition(
    Vector2 innerOffset,
    Vector2 direction,
    Vector2 position
  ) {
    float wiggleSpace = (CellSize - MinWidth) / 2.0f;
    float minWidthOffset = MinWidth / 2.0f;
    return innerOffset * direction * wiggleSpace
      + (position + direction * minWidthOffset);
  }
}