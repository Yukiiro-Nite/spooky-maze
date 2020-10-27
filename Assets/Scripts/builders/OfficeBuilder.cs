using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeBuilder : WallBuilder {
  public float hallWidth = 1f;

  protected static readonly Vector2 nDirection = new Vector2(0, -1);
  protected static readonly Vector2 eDirection = new Vector2(1, 0);
  protected static readonly Vector2 sDirection = new Vector2(0, 1);
  protected static readonly Vector2 wDirection = new Vector2(-1, 0);
  public OfficeBuilder(Maze maze, float CellSize, float MinWidth, float CeilingHeight, float CellPadding): base(maze, CellSize, MinWidth, CeilingHeight, CellPadding) {}

  protected override void BuildCellMesh(Cell cell, Dictionary<string, List<Mesh>> meshes) {
    Vector2 c = new Vector2(cell.x, cell.y) * (CellSize + CellPadding);

    Vector2 v0 = worldPosition(cell.swOffset, swDirection, c);
    Vector2 v1 = worldPosition(cell.nwOffset, nwDirection, c);
    Vector2 v2 = worldPosition(cell.neOffset, neDirection, c);
    Vector2 v3 = worldPosition(cell.seOffset, seDirection, c);

    Vector2 v01 = worldPosition(cell.swOffset, wDirection, c);
    Vector2 v10 = worldPosition(cell.nwOffset, wDirection, c);

    Vector2 v12 = worldPosition(cell.nwOffset, nDirection, c);
    Vector2 v21 = worldPosition(cell.neOffset, nDirection, c);

    Vector2 v23 = worldPosition(cell.neOffset, eDirection, c);
    Vector2 v32 = worldPosition(cell.seOffset, eDirection, c);

    Vector2 v30 = worldPosition(cell.seOffset, sDirection, c);
    Vector2 v03 = worldPosition(cell.swOffset, sDirection, c);

    meshes["floor"].Add(QuadFromPoints(v0, v01, c, v03, 0));
    meshes["floor"].Add(QuadFromPoints(v1, v12, c, v10, 0));
    meshes["floor"].Add(QuadFromPoints(v2, v23, c, v21, 0));
    meshes["floor"].Add(QuadFromPoints(v3, v30, c, v32, 0));

    meshes["ceiling"].Add(QuadFromPoints(v0, v03, c, v01, CeilingHeight));
    meshes["ceiling"].Add(QuadFromPoints(v1, v10, c, v12, CeilingHeight));
    meshes["ceiling"].Add(QuadFromPoints(v2, v21, c, v23, CeilingHeight));
    meshes["ceiling"].Add(QuadFromPoints(v3, v32, c, v30, CeilingHeight));
  }

  protected override void BuildWallMesh(Cell from, string dir, Dictionary<string, List<Mesh>> meshes) {
    Vector2 c = new Vector2(from.x, from.y) * (CellSize + CellPadding);

    Vector2 v0 = worldPosition(from.swOffset, swDirection, c);
    Vector2 v1 = worldPosition(from.nwOffset, nwDirection, c);
    Vector2 v2 = worldPosition(from.neOffset, neDirection, c);
    Vector2 v3 = worldPosition(from.seOffset, seDirection, c);

    Vector2 v01 = worldPosition(from.swOffset, wDirection, c);
    Vector2 v10 = worldPosition(from.nwOffset, wDirection, c);

    Vector2 v12 = worldPosition(from.nwOffset, nDirection, c);
    Vector2 v21 = worldPosition(from.neOffset, nDirection, c);

    Vector2 v23 = worldPosition(from.neOffset, eDirection, c);
    Vector2 v32 = worldPosition(from.seOffset, eDirection, c);

    Vector2 v30 = worldPosition(from.seOffset, sDirection, c);
    Vector2 v03 = worldPosition(from.swOffset, sDirection, c);

    if(dir == "north") {
      meshes["wall"].Add(WallQuad(v1, v12, CeilingHeight, 0f));
      meshes["wall"].Add(WallQuad(v12, v21, CeilingHeight, 0f));
      meshes["wall"].Add(WallQuad(v21, v2, CeilingHeight, 0f));
    } else if(dir == "east") {
      meshes["wall"].Add(WallQuad(v2, v23, CeilingHeight, 0f));
      meshes["wall"].Add(WallQuad(v23, v32, CeilingHeight, 0f));
      meshes["wall"].Add(WallQuad(v32, v3, CeilingHeight, 0f));
    } else if(dir == "south") {
      meshes["wall"].Add(WallQuad(v3, v30, CeilingHeight, 0f));
      meshes["wall"].Add(WallQuad(v30, v03, CeilingHeight, 0f));
      meshes["wall"].Add(WallQuad(v03, v0, CeilingHeight, 0f));
    } else if(dir == "west") {
      meshes["wall"].Add(WallQuad(v0, v01, CeilingHeight, 0f));
      meshes["wall"].Add(WallQuad(v01, v10, CeilingHeight, 0f));
      meshes["wall"].Add(WallQuad(v10, v1, CeilingHeight, 0f));
    }
  }

  protected override void BuildPathMesh(Cell from, Cell to, string dir, Dictionary<string, List<Mesh>> meshes) {
    // important points for from cell.
    Vector2 ac = new Vector2(from.x, from.y) * (CellSize + CellPadding);

    Vector2 a0 = worldPosition(from.swOffset, swDirection, ac);
    Vector2 a1 = worldPosition(from.nwOffset, nwDirection, ac);
    Vector2 a2 = worldPosition(from.neOffset, neDirection, ac);
    Vector2 a3 = worldPosition(from.seOffset, seDirection, ac);

    Vector2 a01 = worldPosition(from.swOffset, wDirection, ac);
    Vector2 a10 = worldPosition(from.nwOffset, wDirection, ac);

    Vector2 a12 = worldPosition(from.nwOffset, nDirection, ac);
    Vector2 a21 = worldPosition(from.neOffset, nDirection, ac);

    Vector2 a23 = worldPosition(from.neOffset, eDirection, ac);
    Vector2 a32 = worldPosition(from.seOffset, eDirection, ac);

    Vector2 a30 = worldPosition(from.seOffset, sDirection, ac);
    Vector2 a03 = worldPosition(from.swOffset, sDirection, ac);

    // important points for to cell
    Vector2 bc = new Vector2(to.x, to.y) * (CellSize + CellPadding);

    Vector2 b0 = worldPosition(to.swOffset, swDirection, bc);
    Vector2 b1 = worldPosition(to.nwOffset, nwDirection, bc);
    Vector2 b2 = worldPosition(to.neOffset, neDirection, bc);
    Vector2 b3 = worldPosition(to.seOffset, seDirection, bc);

    Vector2 b01 = worldPosition(to.swOffset, wDirection, bc);
    Vector2 b10 = worldPosition(to.nwOffset, wDirection, bc);

    Vector2 b12 = worldPosition(to.nwOffset, nDirection, bc);
    Vector2 b21 = worldPosition(to.neOffset, nDirection, bc);

    Vector2 b23 = worldPosition(to.neOffset, eDirection, bc);
    Vector2 b32 = worldPosition(to.seOffset, eDirection, bc);

    Vector2 b30 = worldPosition(to.seOffset, sDirection, bc);
    Vector2 b03 = worldPosition(to.swOffset, sDirection, bc);

    if(dir == "north") {
      BuildOfficePath(ac, a1, a12, a21, a2, bc, b0, b03, b30, b3, nDirection, meshes);
    } else if(dir == "east") {
      BuildOfficePath(ac, a2, a23, a32, a3, bc, b1, b10, b01, b0, eDirection, meshes);
    } else if(dir == "south") {
      BuildOfficePath(ac, a3, a30, a03, a0, bc, b2, b21, b12, b1, sDirection, meshes);
    } else if(dir == "west") {
      BuildOfficePath(ac, a0, a01, a10, a1, bc, b3, b32, b23, b2, wDirection, meshes);
    }
  }

  private void BuildOfficePath(
    Vector2 ac,
    Vector2 a1,
    Vector2 a12,
    Vector2 a21,
    Vector2 a2,
    Vector2 bc,
    Vector2 b0,
    Vector2 b03,
    Vector2 b30,
    Vector2 b3,
    Vector2 dir,
    Dictionary<string, List<Mesh>> meshes
  ) {
    Vector2 up = dir;
    Vector2 left = rotate(dir, -Mathf.PI / 2f);
    Vector2 down = rotate(dir, Mathf.PI);
    Vector2 right = rotate(dir, Mathf.PI / 2f);

    Vector2 upOff = up * hallWidth;
    Vector2 leftOff = left * hallWidth;
    Vector2 downOff = down * hallWidth;
    Vector2 rightOff = right * hallWidth;

    bool LRPath = IsALeftOfB(
      a1 + ((a2-a1) / 2f),
      b0 + ((b3-b0) / 2f),
      up
    );

    if(LRPath) {
      Vector2 h0 = HallPos(ac, up) * Abs(up) + a1 * Abs(right);
      Vector2 h1 = h0 + upOff;
      Vector2 h2 = HallPos(bc, down) * Abs(down) + b3 * Abs(right);
      Vector2 h3 = h2 + downOff;

      Vector2 h03 = h0 + rightOff;
      Vector2 h21 = h2 + leftOff;

      meshes["floor"].Add(QuadFromPoints(a1, h0, h03, a1 + rightOff, 0));
      meshes["floor"].Add(QuadFromPoints(h0, h1, h2, h3, 0));
      meshes["floor"].Add(QuadFromPoints(b3, h2, h21, b3 + leftOff, 0));

      meshes["ceiling"].Add(QuadFromPoints(a1, a1 + rightOff, h03, h0, CeilingHeight));
      meshes["ceiling"].Add(QuadFromPoints(h0, h3, h2, h1, CeilingHeight));
      meshes["ceiling"].Add(QuadFromPoints(b3, b3 + leftOff, h21, h2, CeilingHeight));

      AddWalls(new Vector2[]{
        a1, h1, h21, b3 + leftOff, b30, b03, b0
      }, CeilingHeight, 0, meshes);
      AddWalls(new Vector2[]{
        b3, h3, h03, a1 + rightOff, a12, a21, a2
      }, CeilingHeight, 0, meshes);
    } else {
      Vector2 h3 = HallPos(ac, up) * Abs(up) + a2 * Abs(right);
      Vector2 h2 = h3 + upOff;
      Vector2 h1 = HallPos(bc, down) * Abs(down) + b0 * Abs(right);
      Vector2 h0 = h1 + downOff;

      Vector2 h30 = h3 + leftOff;
      Vector2 h12 = h1 + rightOff;

      meshes["floor"].Add(QuadFromPoints(a2, a2 + leftOff, h30, h3, 0));
      meshes["floor"].Add(QuadFromPoints(h0, h1, h2, h3, 0));
      meshes["floor"].Add(QuadFromPoints(b0, b0 + rightOff, h12, h1, 0));

      meshes["ceiling"].Add(QuadFromPoints(a2, h3, h30, a2 + leftOff, CeilingHeight));
      meshes["ceiling"].Add(QuadFromPoints(h0, h3, h2, h1, CeilingHeight));
      meshes["ceiling"].Add(QuadFromPoints(b0, h1, h12, b0 + rightOff, CeilingHeight));

      AddWalls(new Vector2[]{
        a1, a12, a21, a2 + leftOff, h30, h0, b0
      }, CeilingHeight, 0, meshes);
      AddWalls(new Vector2[]{
        b3, b30, b03, b0 + rightOff, h12, h2, a2
      }, CeilingHeight, 0, meshes);
    }
  }

  private float avg(float a, float b) {
    return (a+b)/2f;
  }

  private Vector2 Abs(Vector2 v) {
    return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
  }

  private Vector2 HallPos(Vector2 c, Vector2 dir) {
    return (c + (dir * (CellSize / 2.0f + CellPadding / 2.0f - hallWidth / 2.0f)));
  }

  private Vector2 rotate(Vector2 v, float angle) {
    float theta = Mathf.Atan2(v.y, v.x);
    float magnitude = v.magnitude;
    float a = theta + angle;
    return new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * magnitude;
  }

  private bool IsALeftOfB(Vector2 a, Vector2 b, Vector2 dir) {
    float rotation = Mathf.PI / 2f - Mathf.Atan2(dir.y, dir.x);
    a = rotate(a, rotation);
    b = rotate(b, rotation);
    return a.x < b.x;
  }
  
  protected override void Floor(Vector2 nearLeft, Vector2 farLeft, Vector2 farRight, Vector2 nearRight, Dictionary<string, List<Mesh>> meshes, Dictionary<string, bool> dir) {

  }

  protected override void Ceiling(Vector2 nearLeft, Vector2 farLeft, Vector2 farRight, Vector2 nearRight, Dictionary<string, List<Mesh>> meshes, Dictionary<string, bool> dir) {
    
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

  private void AddWalls(Vector2[] points, float top, float bottom, Dictionary<string, List<Mesh>> meshes) {
    for(int i=0; i< points.Length-1; i++) {
      meshes["wall"].Add(WallQuad(points[i], points[i+1], CeilingHeight, 0));
    }
  }
}