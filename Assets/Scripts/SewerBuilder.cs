using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewerBuilder : WallBuilder {
  public float channelWidth = 0.5f;
  public float channelDepth = -0.05f;
  public SewerBuilder(Maze maze, float CellSize, float MinWidth, float CeilingHeight, float CellPadding): base(maze, CellSize, MinWidth, CeilingHeight, CellPadding) {}

  public override void BuildMaze(Cell cell, int depth) {
    Dictionary<Cell, int> visited = new Dictionary<Cell, int>();
    Dictionary<string, List<Mesh>> meshes = new Dictionary<string, List<Mesh>>();
    meshes.Add("floor", new List<Mesh>());
    meshes.Add("channel", new List<Mesh>());
    meshes.Add("wall", new List<Mesh>());
    meshes.Add("ceiling", new List<Mesh>());
    
    BuildCell(cell, depth, visited, meshes);
    CreateObject("floors", meshes["floor"], maze.type);
    CreateObject("channels", meshes["channel"], maze.type);
    CreateObject("walls", meshes["wall"], maze.type);
    CreateObject("ceilings", meshes["ceiling"], maze.type);
  }

  protected override void Floor(Vector2 nearLeft, Vector2 farLeft, Vector2 farRight, Vector2 nearRight, Dictionary<string, List<Mesh>> meshes, Dictionary<string, bool> dir) {
    Debug.Log("Calling sewer specific floor builder");
    Channel(nearLeft, farLeft, farRight, nearRight, meshes, dir);
  }

  protected virtual void Channel(Vector2 nearLeft, Vector2 farLeft, Vector2 farRight, Vector2 nearRight, Dictionary<string, List<Mesh>> meshes, Dictionary<string, bool> dir) {
    Vector2 v01 = interpolate(nearLeft, farLeft, channelWidth);
    Vector2 v10 = interpolate(farLeft, nearLeft, channelWidth);

    Vector2 v12 = interpolate(farLeft, farRight, channelWidth);
    Vector2 v21 = interpolate(farRight, farLeft, channelWidth);

    Vector2 v23 = interpolate(farRight, nearRight, channelWidth);
    Vector2 v32 = interpolate(nearRight, farRight, channelWidth);

    Vector2 v30 = interpolate(nearRight, nearLeft, channelWidth);
    Vector2 v03 = interpolate(nearLeft, nearRight, channelWidth);

    Vector2 v0 = intersect(v01, v32, v03, v12);
    Vector2 v1 = intersect(v12, v03, v10, v23);
    Vector2 v2 = intersect(v23, v10, v21, v30);
    Vector2 v3 = intersect(v30, v21, v32, v01);

    // Add corner floor panels and center channel
    meshes["floor"].Add(QuadFromPoints(nearLeft, v01, v0, v03, 0));
    meshes["floor"].Add(QuadFromPoints(farLeft, v12, v1, v10, 0));
    meshes["floor"].Add(QuadFromPoints(farRight, v23, v2, v21, 0));
    meshes["floor"].Add(QuadFromPoints(nearRight, v30, v3, v32, 0));
    meshes["channel"].Add(QuadFromPoints(v0, v1, v2, v3, channelDepth));

    // Then add channels for each direction.
    if(dir["front"]) {
      meshes["channel"].Add(QuadFromPoints(v1, v12, v21, v2, channelDepth));
      meshes["floor"].Add(WallQuad(v1, v12, 0, channelDepth));
      meshes["floor"].Add(WallQuad(v21, v2, 0, channelDepth));
    } else {
      meshes["floor"].Add(QuadFromPoints(v1, v12, v21, v2, 0));
      meshes["floor"].Add(WallQuad(v1, v2, 0, channelDepth));
    }

    if(dir["right"]) {
      meshes["channel"].Add(QuadFromPoints(v3, v2, v23, v32, channelDepth));
      meshes["floor"].Add(WallQuad(v2, v23, 0, channelDepth));
      meshes["floor"].Add(WallQuad(v32, v3, 0, channelDepth));
    } else {
      meshes["floor"].Add(QuadFromPoints(v3, v2, v23, v32, 0));
      meshes["floor"].Add(WallQuad(v2, v3, 0, channelDepth));
    }

    if(dir["back"]) {
      meshes["channel"].Add(QuadFromPoints(v03, v0, v3, v30, channelDepth));
      meshes["floor"].Add(WallQuad(v3, v30, 0, channelDepth));
      meshes["floor"].Add(WallQuad(v03, v0, 0, channelDepth));
    } else {
      meshes["floor"].Add(QuadFromPoints(v03, v0, v3, v30, 0));
      meshes["floor"].Add(WallQuad(v3, v0, 0, channelDepth));
    }

    if(dir["left"]) {
      meshes["channel"].Add(QuadFromPoints(v01, v10, v1, v0, channelDepth));
      meshes["floor"].Add(WallQuad(v0, v01, 0, channelDepth));
      meshes["floor"].Add(WallQuad(v10, v1, 0, channelDepth));
    } else {
      meshes["floor"].Add(QuadFromPoints(v01, v10, v1, v0, 0));
      meshes["floor"].Add(WallQuad(v0, v1, 0, channelDepth));
    }
  }

  private Vector2 interpolate(Vector2 from, Vector2 to, float scale) {
    return from + (to - from) * (scale / 2.0f);
  }

  private Vector2 intersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d) {
    float a1 = b.y - a.y;
    float b1 = a.x - b.x;
    float c1 = a1*a.x + b1*a.y;

    float a2 = d.y - c.y;
    float b2 = c.x - d.x;
    float c2 = a2*c.x + b2*c.y;

    float det = a1 * b2 - a2 * b1;

    if(det == 0) {
      return a;
    } else {
      return new Vector2(
        (b2 * c1 - b1 * c2) / det,
        (a1 * c2 - a2 * c1) / det
      );
    }
  }

  private Mesh QuadFromPoints(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float height) {
    return QuadFromPoints(
      new Vector3(p0.x, height, p0.y),
      new Vector3(p1.x, height, p1.y),
      new Vector3(p2.x, height, p2.y),
      new Vector3(p3.x, height, p3.y)
    );
  }

  private Mesh WallQuad(Vector2 p0, Vector2 p1, float top, float bottom) {
    return QuadFromPoints(
      new Vector3(p0.x, top, p0.y),
      new Vector3(p1.x, top, p1.y),
      new Vector3(p1.x, bottom, p1.y),
      new Vector3(p0.x, bottom, p0.y)
    );
  }
}