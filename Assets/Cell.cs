using System;
using Random = UnityEngine.Random;
using UnityEngine;
public class Cell {
  public readonly int x;
  public readonly int y;
  public bool north;
  public bool east;
  public bool south;
  public bool west;
  public bool visited;

  // Offsets determine corner positions inside of each cell.
  // Used for building maze walls.
  public readonly Vector2 neOffset;
  public readonly Vector2 seOffset;
  public readonly Vector2 swOffset;
  public readonly Vector2 nwOffset;
  public Cell(int x, int y) {
    this.x = x;
    this.y = y;

    this.neOffset = new Vector2((float)Random.value, (float)Random.value);
    this.seOffset = new Vector2((float)Random.value, (float)Random.value);
    this.swOffset = new Vector2((float)Random.value, (float)Random.value);
    this.nwOffset = new Vector2((float)Random.value, (float)Random.value);
  }

  public void open(string dir) {
    if(dir.Equals("north")) {
      north = true;
    } else if(dir.Equals("east")) {
      east = true;
    } else if(dir.Equals("south")) {
      south = true;
    } else if(dir.Equals("west")) {
      west = true;
    }
  }

  public override bool Equals(object other) {
    return Equals(other as Cell);
  }

  public virtual bool Equals(Cell other) {
    if (other == null) { return false; }
    if (object.ReferenceEquals(this, other)) { return true; }
    return this.x == other.x && this.y == other.y;
  }

  public override int GetHashCode() {
    return Tuple.Create(this.x, this.y).GetHashCode();
  }

  public static bool operator ==(Cell cell1, Cell cell2) {
    if (object.ReferenceEquals(cell1, cell2)) { return true; }
    if ((object)cell1 == null || (object)cell2 == null) { return false; }
    return cell1.x == cell2.x && cell1.y == cell2.y;
  }

  public static bool operator !=(Cell cell1, Cell cell2) {
    return !(cell1 == cell2);
  }
}