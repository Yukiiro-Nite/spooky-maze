using System;
using UnityEngine;
public class Wall {
  public readonly Vector2 P1;
  public readonly Vector2 P2;
  public Wall(Vector2 P1, Vector2 P2) {
    this.P1 = P1;
    this.P2 = P2;
  }

  public Wall(float x1, float y1, float x2, float y2) {
    this.P1 = new Vector2(x1, y1);
    this.P2 = new Vector2(x2, y2);
  }

  public override bool Equals(object other) {
    return Equals(other as Wall);
  }

  public virtual bool Equals(Wall other) {
    if (other == null) { return false; }
    if (object.ReferenceEquals(this, other)) { return true; }
    return this.P1 == other.P1 && this.P2 == other.P2
      ||  this.P1 == other.P2 && this.P2 == other.P1;
  }

  public override int GetHashCode() {
    return Tuple.Create(P1.x, P1.y).GetHashCode()
      ^ Tuple.Create(P2.x, P2.y).GetHashCode();
  }

  public static bool operator ==(Wall wall1, Wall wall2) {
    if (object.ReferenceEquals(wall1, wall2)) { return true; }
    if ((object)wall1 == null || (object)wall2 == null) { return false; }
    return wall1.P1 == wall2.P1 && wall1.P2 == wall2.P2
      ||  wall1.P1 == wall2.P2 && wall1.P2 == wall2.P1;
  }

  public static bool operator !=(Wall wall1, Wall wall2) {
    return !(wall1 == wall2);
  }
}