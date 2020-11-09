using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour {
  NavGrid grid;

  void Awake() {
    grid = GetComponent<NavGrid>();
  }

  public List<NavNode> FindPath(Vector3 startPos, Vector3 endPos) {
    NavNode startNode = grid.NodeFromWorldPoint(startPos);
    NavNode endNode = grid.NodeFromWorldPoint(endPos);
    int iterationCount = 0;
    int maxIterations = 10000;

    SortedSet<NavNode> openSet = new SortedSet<NavNode>(new NavNodeComparer());
    HashSet<NavNode> closedSet = new HashSet<NavNode>();
    openSet.Add(startNode);

    NavNode currentNode;
    while(openSet.Count > 0 && iterationCount <= maxIterations) {
      iterationCount++;

      currentNode = openSet.Min;
      openSet.Remove(currentNode);
      closedSet.Add(currentNode);

      if(currentNode == endNode) {
        return TracePath(startNode, endNode);
      }

      foreach(NavNode neighbor in grid.GetNeighbors(currentNode)) {
        if(closedSet.Contains(neighbor)) {
          continue;
        }

        float toNeighborCost = currentNode.fromStartCost + GetDistance(currentNode, neighbor);
        if(toNeighborCost < neighbor.fromStartCost || !openSet.Contains(neighbor)) {
          // When using SortedSet, we need to remove the item before updating it.
          openSet.Remove(neighbor);

          neighbor.fromStartCost = toNeighborCost;
          neighbor.toEndCost = GetDistance(neighbor, endNode);
          neighbor.parent = currentNode;

          openSet.Add(neighbor);
        }
      }
    }

    if(iterationCount == maxIterations) {
      Debug.Log("FindPath exited due to too many iterations.");
    }

    return new List<NavNode>();
  }

  public List<NavNode> TracePath(NavNode startNode, NavNode endNode) {
    List<NavNode> path = new List<NavNode>();
    NavNode currentNode = endNode;

    while(currentNode != startNode && currentNode != null) {
      path.Add(currentNode);
      currentNode = currentNode.parent;
    }

    path.Reverse();
    return path;
  }

  float GetDistance(NavNode a, NavNode b) {
    return Vector3.Distance(a.worldPos, b.worldPos);
  }
}