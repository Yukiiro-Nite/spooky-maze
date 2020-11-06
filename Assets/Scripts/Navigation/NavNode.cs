using UnityEngine;
using System;
using System.Collections;

public class NavNode : IComparable {
  public bool walkable;
  public Vector2Int gridPos;
  public Vector3 worldPos;
  public float fromStartCost;
  public float toEndCost;
  public NavNode parent;

  public float totalCost {
    get {
      float walkableCoefficient = walkable
        ? 1
        : 1000;
      return walkableCoefficient * (fromStartCost + toEndCost);
    }
  }

  public NavNode(bool walkable, Vector2Int gridPos, Vector3 worldPos) {
    this.walkable = walkable;
    this.gridPos = gridPos;
    this.worldPos = worldPos;
  }

  public int CompareTo(object obj) {
    if(obj == null) return -1;

    NavNode other = obj as NavNode;
    if(other != null) {
      int compare = totalCost.CompareTo(other.totalCost);
      if (compare == 0) {
        compare = toEndCost.CompareTo(other.toEndCost);
      }
      return compare;
    } else {
      throw new ArgumentException("Object is not a NavNode");
    }
	}
}