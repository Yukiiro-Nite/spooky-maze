using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavGrid : MonoBehaviour {
  public bool showGizmos;
  private bool lastShowGizmos;
  public LayerMask walkableMask;
  public LayerMask unwalkableMask;
  public Vector2 gridWorldSize;
  public float nodeRadius;
  public GameObject unit;
  protected Camera target;
  NavNode[,] grid;
  float nodeDiameter {
    get {
      return nodeRadius * 2;
    }
  }
  Vector2Int gridSize;
  private MapManager mapManager;
  private Pathfinder pathfinder;
  private List<NavNode> path;

  void Start() {
    mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
    pathfinder = GetComponent<Pathfinder>();
    target = Camera.main;
  }

  void Update() {
    if(!lastShowGizmos && showGizmos) {
      path = pathfinder.FindPath(unit.transform.position, target.transform.position);
    }
    
    lastShowGizmos = showGizmos;
  }

  public void CreateGrid() {
    mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
    gridWorldSize = new Vector2(
      mapManager.Width * (mapManager.CellSize + mapManager.CellPadding),
      mapManager.Height * (mapManager.CellSize + mapManager.CellPadding)
    );
    gridSize = new Vector2Int(
      Mathf.RoundToInt(gridWorldSize.x / nodeDiameter),
      Mathf.RoundToInt(gridWorldSize.y / nodeDiameter)
    );
    // Should be called after map manager has generated the maze.
    grid = new NavNode[gridSize.x, gridSize.y];

    for(int x = 0; x < gridSize.x; x++) {
      for(int y = 0; y < gridSize.y; y++) {
        Vector3 worldPoint = Vector3.right * (x * nodeDiameter + nodeRadius)
          + Vector3.forward * (y * nodeDiameter + nodeRadius)
          - new Vector3(mapManager.CellSize / 2f, 0, mapManager.CellSize / 2f);
        bool walkable = Physics.CheckSphere(worldPoint, nodeRadius, walkableMask)
          && !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
        grid[x,y] = new NavNode(walkable, new Vector2Int(x,y), worldPoint);
      }
    }

    Debug.Log("Finished generating nav grid.");
  }

  public List<NavNode> GetNeighbors(NavNode node) {
    List<NavNode> neighbors = new List<NavNode>();
    int checkX, checkY;
    bool inGrid;

    for(int x = -1; x <= 1; x++) {
      for(int y = -1; y <= 1; y++) {
        if(x == 0 && y == 0) {
          continue;
        } else {
          checkX = node.gridPos.x + x;
          checkY = node.gridPos.y + y;
          inGrid = checkX >= 0 && checkX < gridSize.x
            && checkY >= 0 && checkY < gridSize.y;
          
          if(inGrid) {
            neighbors.Add(grid[checkX, checkY]);
          }
        }
      }
    }

    return neighbors;
  }

  public NavNode NodeFromWorldPoint(Vector3 worldPosition) {
    float percentX = (worldPosition.x + mapManager.CellSize / 2f) / gridWorldSize.x;
    float percentY = (worldPosition.z + mapManager.CellSize / 2f) / gridWorldSize.y;
    percentX = Mathf.Clamp01(percentX);
    percentY = Mathf.Clamp01(percentY);
    
    int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
    int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);

    return grid[x, y];
  }

  void OnDrawGizmos() {
    if(mapManager != null) {
      Vector3 gridCenter = transform.position
        + new Vector3(gridWorldSize.x / 2f, 0, gridWorldSize.y / 2f)
        - new Vector3(mapManager.CellSize / 2f, 0, mapManager.CellSize / 2f);
      Gizmos.DrawWireCube(gridCenter, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

      if(grid != null && showGizmos) {
        NavNode unitNode = NodeFromWorldPoint(unit.transform.position);
        foreach(NavNode n in grid) {
          Gizmos.color = n.walkable
            ? Color.white
            : Color.red;
          if(path != null && path.Contains(n)) {
            Gizmos.color = Color.green;
          }
          if(n == unitNode) {
            Gizmos.color = Color.cyan;
          }
          Gizmos.DrawWireCube(n.worldPos, Vector3.one * (nodeDiameter - 0.1f));
        }
      }
    }
  }
}