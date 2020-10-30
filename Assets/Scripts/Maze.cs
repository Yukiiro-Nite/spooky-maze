using System;
using UnityEngine;
using System.Collections.Generic;
public class Maze {
  public int width;
  public int height;
  public string type;
  public Vector2 start;
  public Vector2 end;
  private Cell[,] cells;

  public Maze (int width, int height, string type) {
    this.width = width;
    this.height = height;
    this.type = type;

    cells = new Cell[this.height, this.width];

    for (int y = 0; y < this.height; y++) {
      for (int x = 0; x < this.width; x++) {
        cells[y, x] = new Cell(x, y);
      }
    }
  }

  public Cell getCell(Vector2 pos) {
    int x = (int)Mathf.Round(pos.x);
    int y = (int)Mathf.Round(pos.y);
    if(x >= 0 && x < width && y >= 0 && y < height) {
      return cells[y, x];
    } else {
      return null;
    }
  }

  public Cell getCell(int x, int y) {
    if(x >= 0 && x < width && y >= 0 && y < height) {
      return cells[y, x];
    } else {
      return null;
    }
  }

  public Vector2 offsetPosition(int x, int y, string dir) {
    if(dir.Equals("north")) {
      return new Vector2(x, y - 1);
    } else if(dir.Equals("east")) {
      return new Vector2(x + 1, y);
    } else if(dir.Equals("south")) {
      return new Vector2(x, y + 1);
    } else if(dir.Equals("west")) {
      return new Vector2(x - 1, y);
    } else {
      return new Vector2();
    }
  }

  public string oppositeDirection(string dir) {
    if(dir.Equals("north")) {
      return "south";
    } else if(dir.Equals("east")) {
      return "west";
    } else if(dir.Equals("south")) {
      return "north";
    } else if(dir.Equals("west")) {
      return "east";
    } else {
      return null;
    }
  }

  public string directionFromVector(int x, int y) {
    if(x == 0 && y == -1) {
      return "north";
    } else if(x == 1 && y == 0) {
      return "east";
    } else if(x == 0 && y == 1) {
      return "south";
    } else if(x == -1 && y == 0) {
      return "west";
    } else {
      return null;
    }
  }

  public bool openWall(int x, int y, string dir) {
    Cell currentCell = getCell(x, y);
    Vector2 offset = offsetPosition(x, y, dir);
    Cell neighborCell = getCell((int)offset.x, (int)offset.y);

    if(currentCell != null && neighborCell != null) {
      currentCell.open(dir);
      neighborCell.open(oppositeDirection(dir));

      return true;
    } else {
      return false;
    }
  }

  public bool openWall(Cell cell1, Cell cell2) {
    int xDiff = cell2.x - cell1.x;
    int yDiff = cell2.y - cell1.y;
    string dir = directionFromVector(xDiff, yDiff);

    if(dir != null) {
      return openWall(cell1.x, cell1.y, dir);
    } else {
      return false;
    }
  }

  public bool IsConnected(Cell cell1, Cell cell2, string direction) {
    if(cell1 == null || cell2 == null) {
      return false;
    } else {
      return (bool)cell1.GetType().GetField(direction).GetValue(cell1) &&
        (bool)cell2.GetType().GetField(oppositeDirection(direction)).GetValue(cell2);
    }
  }

  public bool hasUnvisitedCells() {
    for (int y = 0; y < this.height; y++) {
      for (int x = 0; x < this.width; x++) {
        if(!cells[y, x].visited) {
          return true;
        }
      }
    }

    return false;
  }

  public Cell getNeighbor(Cell cell, string direction) {
    Vector2 offset = offsetPosition(cell.x, cell.y, direction);
    return getCell((int)offset.x, (int)offset.y);
  }

  public bool hasUnvisitedNeighbors(Cell cell) {
    List<Cell> neighbors = getNeighbors(cell);

    return neighbors.Exists(neighbor => !neighbor.visited);
  }

  public List<Cell> getUnvisitedNeighbors(Cell cell) {
    List<Cell> neighbors = getNeighbors(cell);

    return neighbors.FindAll(neighbor => !neighbor.visited);
  }

  public List<Cell> getNeighbors(Cell cell) {
    string[] directions = { "north", "east", "south", "west" };
    List<Cell> neighbors = new List<Cell>();

    foreach(string direction in directions) {
      Cell neighbor = getNeighbor(cell, direction);
      if(neighbor != null) {
        neighbors.Add(neighbor);
      }
    }

    return neighbors;
  }

  public List<Cell> getConnectedNeighbors(Cell cell) {
    string[] directions = { "north", "east", "south", "west" };
    List<Cell> neighbors = new List<Cell>();

    foreach(string direction in directions) {
      Cell neighbor = getNeighbor(cell, direction);
      if(neighbor != null && IsConnected(cell, neighbor, direction)) {
        neighbors.Add(neighbor);
      }
    }

    return neighbors;
  }

  public HashSet<Cell> Neighborhood(int x, int y, int depth) {
    return Neighborhood(getCell(x, y), depth);
  }

  public HashSet<Cell> Neighborhood(Cell cell, int depth) {
    HashSet<Cell> cells = new HashSet<Cell>();
    cells.Add(cell);

    if(depth != 0) {
      List<Cell> neighbors = getConnectedNeighbors(cell);
      
      foreach(Cell neighbor in neighbors) {
        cells.UnionWith(Neighborhood(neighbor, depth-1));
      }
    }

    return cells;
  }
}