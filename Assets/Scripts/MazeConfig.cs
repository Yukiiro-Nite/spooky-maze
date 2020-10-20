public struct MazeConfig {
  public int Width { get; }
  public int Height { get; }
  public float CeilingHeight { get; }
  public float CellSize { get; }
  public float MinWidth { get; }
  public float CellPadding { get; }
  public int RenderDepth { get; }
  public MazeConfig(int Width, int Height, float CeilingHeight, float CellSize, float MinWidth, float CellPadding, int RenderDepth) {
    this.Width = Width;
    this.Height = Height;
    this.CeilingHeight = CeilingHeight;
    this.CellSize = CellSize;
    this.MinWidth = MinWidth;
    this.CellPadding = CellPadding;
    this.RenderDepth = RenderDepth;
  }
}