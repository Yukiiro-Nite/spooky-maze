using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Security.Cryptography;
public class MazeGenerator {
  public static Maze generate(int width, int height, string type) {
    Maze maze = new Maze(width, height, type);

    // implement maze generation algorithem here.
    int startX = Random.Range(0, width);
    int startY = Random.Range(0, height);

    int endX = Random.Range(0, width);
    int endY = Random.Range(0, height);

    maze.start = new Vector2((int)startX, (int)startY);
    maze.end = new Vector2((int)endX, (int)endY);

    int iterationCount = 0;
    Stack<Cell> visitedCells = new Stack<Cell>();
    Cell currentCell = maze.getCell(startX, startY);
    currentCell.visited = true;
    visitedCells.Push(currentCell);

    while(maze.hasUnvisitedCells() && iterationCount < width * height * 2) {
      if(maze.hasUnvisitedNeighbors(currentCell)) {
        List<Cell> unvisitedNeighbors = maze.getUnvisitedNeighbors(currentCell);
        Shuffle(unvisitedNeighbors);
        int randomIndex = Random.Range(0, unvisitedNeighbors.Count);
        Cell randomNeighbor = unvisitedNeighbors[randomIndex];

        visitedCells.Push(randomNeighbor);
        maze.openWall(currentCell, randomNeighbor);
        randomNeighbor.visited = true;
        currentCell = randomNeighbor;
        iterationCount++;
      } else if (visitedCells.Count != 0) {
        currentCell = visitedCells.Pop();
      }
    }

    if(iterationCount >= width * height * 10) {
      throw new Exception("Too many iterations");
    }

    return maze;
  }

  public static void Shuffle(List<Cell> list) {
    RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
    int n = list.Count;
    while (n > 1)
    {
      byte[] box = new byte[1];
      do provider.GetBytes(box);
      while (!(box[0] < n * (Byte.MaxValue / n)));
      int k = (box[0] % n);
      n--;
      Cell value = list[k];
      list[k] = list[n];
      list[n] = value;
    }
  }
}